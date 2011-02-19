using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EventStore;
using EventStore.Persistence.RavenPersistence;
using EventStore.Serialization;
using Raven.Client;
using Raven.Client.Document;

namespace Ncqrs.Eventing.Storage.JOliver.RavenPersistence
{
    public class AbsoluteOrderingRavenPersistenceEngine : RavenPersistenceEngine, IPersistStreamsWithAbsoluteOrdering
    {
        private readonly IDocumentStore _store;
        private readonly ISerialize _serializer;
        private readonly HiLoKeyGenerator _hiLoGen;
        private static readonly DocumentConvention HiLoConvention = new DocumentConvention
                        {
                            IdentityPartsSeparator = ""
                        };

        public AbsoluteOrderingRavenPersistenceEngine(IDocumentStore store, ISerialize serializer, bool consistentQueries)
            : base(store, serializer, consistentQueries)
        {
            _store = store;
            _serializer = serializer;
            _hiLoGen = new HiLoKeyGenerator(store, "", 1000);            
        }

        public override void Commit(Commit attempt)
        {
            AppendToSequence(attempt);
            base.Commit(attempt);
        }

        private void AppendToSequence(Commit attempt)
        {
            using (var session = _store.OpenSession())
            {
                session.Store(new RavenCommitSequence
                                  {
                                      Commit = new RavenCommitReference {Id = attempt.ToRavenCommitId()},
                                      Sequence = GetNextSequenceNumber(),
                                      Id = GetSequenceId(attempt.CommitId)
                                  });
                session.SaveChanges();
            }
        }

        private long GetNextSequenceNumber()
        {
            return long.Parse(_hiLoGen.GenerateDocumentKey(HiLoConvention, null));
        }

        public IEnumerable<Commit> Fetch(long mostRecentSequentialId, int maxCount)
        {
            using (var session = _store.OpenSession())
            {
                var sequences = session.Query<RavenCommitSequence, RavenCommitSequencesBySequence>()
                    .Customize(x => x.Include<RavenCommitSequence>(p => p.Commit.Id))
                    .Where(x => x.Sequence >= mostRecentSequentialId)
                    .Take(maxCount);
                return sequences
                    .Select(x => session.Load<RavenCommit>(x.Commit.Id))
                    .Select(x => x.ToCommit(_serializer));
            }
        }

        public long GetLastProcessedSequentialNumber(string pipelineName)
        {
            using (var session = _store.OpenSession())
            {
                var state = session.Load<PipelineState>(pipelineName);
                if (state != null)
                {
                    return state.Sequence;
                }
                return 0;
            }
        }

        public void MarkLastProcessed(string pipelineName, Guid lastProcessedCommitSource, Guid lastProcessedCommitId)
        {
            using (var session = _store.OpenSession())
            {
                var sequence = session.Load<RavenCommitSequence>(GetSequenceId(lastProcessedCommitId));
                var state = session.Load<PipelineState>(pipelineName)
                            ?? new PipelineState
                                   {
                                       Id = pipelineName,
                                       Sequence = sequence.Sequence
                                   };
                session.Store(state);
                session.SaveChanges();
            }
        }

        private static string GetSequenceId(Guid commitId)
        {
            return commitId.ToString("D");
        }
    }
}

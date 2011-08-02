using System.Linq;
using Ncqrs.Commanding.CommandExecution.Mapping.Fluent;
using Raven.Client.Indexes;

namespace Ncqrs.Eventing.Storage.JOliver.RavenPersistence
{
    public class RavenCommitSequencesBySequence : AbstractIndexCreationTask<RavenCommitSequence>
    {
        public RavenCommitSequencesBySequence()
        {
            Map = commitSequences => from c in commitSequences
                                     select new { c.Sequence };
        }
    }
}
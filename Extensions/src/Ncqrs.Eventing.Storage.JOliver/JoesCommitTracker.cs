using System;
using System.Collections.Generic;

namespace Ncqrs.Eventing.Storage.JOliver
{
    public class JoesCommitTracker
    {
        [ThreadStatic]
        private static Dictionary<Guid, int> _commits;

        private static Dictionary<Guid, int> Commits
        {
            get
            {
                if (_commits == null)
                {
                    _commits = new Dictionary<Guid, int>();
                }
                return _commits;
            }
        }

        public void TrackCommitSequence(Guid aggregateId, int commitSequnce)
        {
            Commits[aggregateId] = commitSequnce;
        }

        public int GetCommitSequence(Guid aggregateId)
        {
            int result; 
            if (!Commits.TryGetValue(aggregateId, out result))
            {
                throw new InvalidOperationException("Cannot get commit sequence of a non-tracked aggregate root.");
            }
            Commits.Remove(aggregateId);
            return result;
        }
    }
}
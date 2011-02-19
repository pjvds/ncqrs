using System;

namespace Ncqrs.Eventing.Storage.JOliver.RavenPersistence
{
    public class RavenCommitSequence
    {
        public string Id { get; set; }
        public RavenCommitReference Commit { get; set; }
        public long Sequence { get; set; }
    }
}
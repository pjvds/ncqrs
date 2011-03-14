namespace Ncqrs.Eventing.Storage.JOliver.RavenPersistence
{
    public class PipelineState
    {
        public string Id { get; set; }
        public long Sequence { get; set; }
    }
}
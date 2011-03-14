namespace Ncqrs.EventBus.Tests
{
    public class FakeProcessingElement : IProcessingElement
    {        
        public int SequenceNumber { get; set; }
        public string UniqueId { get; set; }
        public object GroupingKey { get; set; }
    }
}
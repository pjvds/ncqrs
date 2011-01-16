namespace Ncqrs.EventBus
{
    public interface IProcessingElement
    {
        int SequenceNumber { get; set; }
        string UniqueId { get; }
        object GroupingKey { get; }
    }
}
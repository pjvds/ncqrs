namespace Ncqrs.EventBus
{
    public interface IElementProcessor
    {
        void Process(IProcessingElement evnt);
    }
}
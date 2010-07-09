using Newtonsoft.Json.Linq;

namespace Ncqrs.Eventing.Storage.Serialization
{
    /// <summary>
    /// An event converter that does nothing.
    /// </summary>
    /// <remarks>
    /// This is useful when you have to provide an event converter
    /// but do not need to upgrade events to newer versions.
    /// </remarks>
    public class NullEventConverter : IEventConverter
    {
        public void Upgrade(StoredEvent<JObject> theEvent)
        {
            //do nothing
        }
    }
}

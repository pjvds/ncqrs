using System;
using Newtonsoft.Json.Linq;

namespace Ncqrs.Eventing.Storage.Serialization
{
    public class NullEventConverter : IEventConverter
    {
        public void Upgrade(StoredEvent<JObject> theEvent)
        {
            //do nothing
        }
    }
}

using System;
using System.Collections.Generic;
using Ncqrs.Eventing.Storage;
using Ncqrs.Eventing.Storage.Serialization;
using Newtonsoft.Json.Linq;

namespace AwesomeAppRefactored.Events
{
    public class PersonCreatedEventPostConverter : IEventConverter
    {
        private static readonly Version V2 = new Version(2, 0);

        public void Upgrade(StoredEvent<JObject> theEvent)
        {
            if (theEvent.EventVersion < V2) {
                var obj = theEvent.Data;

                var name = string.Format("{0} {1}",
                    obj.Property("Forename").Value,
                    obj.Property("Surname").Value);

                obj.Remove("Forename");
                obj.Remove("Surname");
                obj.Add("Name", name);

                theEvent.EventVersion = V2;
            }
        }
    }
}

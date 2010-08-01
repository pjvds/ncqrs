using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Storage.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ncqrs.Eventing.Storage.NoDB
{
    public class NoDBEventStore : IEventStore
    {
        private readonly string _path;
        private JsonEventFormatter _formatter;

        public NoDBEventStore(string path)
        {
            _path = path;
            _formatter = new JsonEventFormatter(new SimpleEventTypeResolver());
        }

        public IEnumerable<SourcedEvent> GetAllEvents(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SourcedEvent> GetAllEventsSinceVersion(Guid id, long version)
        {
            throw new NotImplementedException();
        }

        public void Save(IEventSource source)
        {
            var foldername = source.EventSourceId.ToString().Substring(0, 2);
            var filename = source.EventSourceId.ToString().Substring(2);
            if (!Directory.Exists(Path.Combine(_path, foldername)))
                Directory.CreateDirectory(Path.Combine(_path, foldername));
            using (var writer = new StreamWriter(File.Open(Path.Combine(_path, foldername, filename), FileMode.OpenOrCreate)))
            {
                foreach (var sourcedEvent in source.GetUncommittedEvents())
                {
                    var storedEvent = _formatter.Serialize(sourcedEvent);
                    writer.WriteLine(storedEvent.WriteLine()); 
                }
            }
        }
    }

    public static class StoredEventExtensions
    {
        public static string WriteLine(this StoredEvent<JObject> storedEvent)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0};{1};{2};{3};{4};{5};{6}",
                            storedEvent.EventIdentifier,storedEvent.EventTimeStamp.Ticks, storedEvent.EventName,
                            storedEvent.EventVersion, storedEvent.EventSourceId, storedEvent.EventSequence, 
                            storedEvent.Data.ToString().Replace("\n", "").Replace("\r",""));
            return sb.ToString();
        }

        public static StoredEvent<JObject> ReadStoredEvent(this string eventString)
        {
            var data = eventString.Split(';');
            return new StoredEvent<JObject>(new Guid(data[0]), new DateTime(long.Parse(data[1])), data[2], new Version(data[3]), new Guid(data[4]), long.Parse(data[5]), JObject.Parse(data[6]));
        }
    }
}

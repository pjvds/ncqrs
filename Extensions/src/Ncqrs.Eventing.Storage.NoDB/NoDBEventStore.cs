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
            var file = GetEventSourceFileInfo(id);
            if (!file.Exists) yield break;
            using (var reader = file.OpenText())
            {
                var line = reader.ReadLine();
                while (line != null)
                {
                    yield return (SourcedEvent) _formatter.Deserialize(line.ReadStoredEvent());
                    line = reader.ReadLine();
                }
            }
        }

        public IEnumerable<SourcedEvent> GetAllEventsSinceVersion(Guid id, long version)
        {
            throw new NotImplementedException();
        }

        public void Save(IEventSource source)
        {
            var file = GetEventSourceFileInfo(source.EventSourceId);
            if (!file.Exists && !file.Directory.Exists)
                file.Directory.Create();
            using (var writer = file.AppendText())
            {
                foreach (var sourcedEvent in source.GetUncommittedEvents())
                {
                    var storedEvent = _formatter.Serialize(sourcedEvent);
                    writer.WriteLine(storedEvent.WriteLine()); 
                }
            }
        }

        private FileInfo GetEventSourceFileInfo(Guid eventSourceId)
        {
            var foldername = eventSourceId.ToString().Substring(0, 2);
            var filename = eventSourceId.ToString().Substring(2);
            var path = Path.Combine(_path, foldername, filename);
            return new FileInfo(path);
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

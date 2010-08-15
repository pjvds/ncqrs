using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Storage.Serialization;
using Newtonsoft.Json.Linq;

namespace Ncqrs.Eventing.Storage.NoDB
{
    public class NoDBEventStore : IEventStore
    {
        private readonly JsonEventFormatter _formatter;
        private readonly string _path;

        public NoDBEventStore(string path)
        {
            _path = path;
            _formatter = new JsonEventFormatter(new SimpleEventTypeResolver());
        }

        #region IEventStore Members

        public IEnumerable<SourcedEvent> GetAllEvents(Guid id)
        {
            return GetAllEventsSinceVersion(id, 0);
        }

        public IEnumerable<SourcedEvent> GetAllEventsSinceVersion(Guid id, long version)
        {
            FileInfo file = id.GetEventStoreFileInfo(_path);
            if (!file.Exists) yield break;
            id.GetReadLock();
            var lines = File.ReadLines(file.FullName).ToArray();
            for (int i = 0; i < lines.Length; i++)
            {
                if (i+1 > version)
                    yield return (SourcedEvent)_formatter.Deserialize(lines[i].ReadStoredEvent(id, i+1));
            }
            id.ReleaseReadLock();
        }

        public void Save(IEventSource source)
        {
            FileInfo file = source.EventSourceId.GetEventStoreFileInfo(_path);
            if (!file.Exists && !file.Directory.Exists)
                file.Directory.Create();
            source.EventSourceId.GetWriteLock();
            if (file.Exists)
            {
                if (GetVersion(source.EventSourceId) > source.InitialVersion)
                    throw new ConcurrencyException(source.EventSourceId, source.Version);
            }
            using (var writer = file.AppendText())
            {
                writer.AutoFlush = false;
                foreach (SourcedEvent sourcedEvent in source.GetUncommittedEvents())
                {
                    StoredEvent<JObject> storedEvent = _formatter.Serialize(sourcedEvent);
                    writer.WriteLine(storedEvent.WriteLine());
                }
                writer.Flush();
                UpdateEventSourceVersion(source.EventSourceId, source.Version);
            }
            source.EventSourceId.ReleaseWriteLock();
        }

        private void UpdateEventSourceVersion(Guid id, long version)
        {
            var file = id.GetVersionFile(_path);
            var versionstring = version.ToString("00000000000000000000");
            File.WriteAllText(file.FullName, versionstring);
        }

        private long GetVersion(Guid id)
        {
            var file = id.GetVersionFile(_path);
            var version = File.ReadAllText(file.FullName);
            return long.Parse(version);
        }

        #endregion


    }
}
using System;
using System.Collections.Generic;
using System.IO;
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
            using (StreamReader reader = file.OpenText())
            {
                reader.ReadLine(); //Throw away the version line
                string line = reader.ReadLine();
                int i = 1;
                while (line != null)
                {
                    if (i >= version)
                        yield return (SourcedEvent) _formatter.Deserialize(line.ReadStoredEvent(id, i++));
                    line = reader.ReadLine();
                }
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
                if (GetVersion(file) > source.InitialVersion)
                    throw new ConcurrencyException(source.EventSourceId, source.Version);
            }
            using (var writer = new StreamWriter(file.OpenWrite()))
            {
                writer.AutoFlush = false;
                UpdateEventSourceVersion(writer, source.Version);
                foreach (SourcedEvent sourcedEvent in source.GetUncommittedEvents())
                {
                    StoredEvent<JObject> storedEvent = _formatter.Serialize(sourcedEvent);
                    writer.WriteLine(storedEvent.WriteLine());
                }
                writer.Flush();
            }
            source.EventSourceId.ReleaseWriteLock();
        }

        private void UpdateEventSourceVersion(StreamWriter writer, long version)
        {
            var versionstring = version.ToString("00000000000000000000") + writer.NewLine;
            Stream filestream = writer.BaseStream;
            filestream.Position = 0;
            filestream.Write(Encoding.UTF8.GetBytes(versionstring), 0, versionstring.Length);
            filestream.Position = filestream.Length;
        }

        private static long GetVersion(FileInfo file)
        {
            using (var reader = file.OpenText())
            {
                string readLine = reader.ReadLine();
                return long.Parse(readLine);
            }
        }

        #endregion


    }
}
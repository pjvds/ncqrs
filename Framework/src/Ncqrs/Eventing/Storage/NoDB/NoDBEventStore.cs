using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public IEnumerable<ISourcedEvent> GetAllEvents(Guid id)
        {
            return GetAllEventsSinceVersion(id, 0);
        }

        public IEnumerable<ISourcedEvent> GetAllEventsSinceVersion(Guid id, long version)
        {
            FileInfo file = id.GetEventStoreFileInfo(_path);
            if (!file.Exists || GetVersion(id) <= version) yield break;
            try
            {
                id.GetReadLock();
                using (var reader = file.OpenRead())
                {
                    var indexBuf = new byte[4];
                    reader.Seek(GetEventSourceIndexForVersion(id, version), SeekOrigin.Begin);
                    var curVer = version + 1;
                    while (reader.Read(indexBuf, 0, 4) == 4)
                    {
                        var length = BitConverter.ToInt32(indexBuf, 0);
                        var eventBytes = new byte[length];
                        reader.Read(eventBytes, 0, length);
                        yield return (SourcedEvent)_formatter.Deserialize(eventBytes.ReadStoredEvent(id, curVer++));
                    }
                }
            }
            finally
            {
                id.ReleaseReadLock();
            }
        }

        public void Save(IEventSource source)
        {
            FileInfo file = source.EventSourceId.GetEventStoreFileInfo(_path);
            if (!file.Exists && !file.Directory.Exists)
                file.Directory.Create();
            try
            {
                source.EventSourceId.GetWriteLock();
                if (file.Exists)
                {
                    if (GetVersion(source.EventSourceId) > source.InitialVersion)
                    {
                        throw new ConcurrencyException(source.EventSourceId, source.Version);
                    }
                }
                using (var writer = file.OpenWrite())
                {
                    writer.Seek(0, SeekOrigin.End);
                    var indicies = new long[source.GetUncommittedEvents().Count()];
                    var i = 0;
                    var index = writer.Position;
                    foreach (SourcedEvent sourcedEvent in source.GetUncommittedEvents())
                    {
                        StoredEvent<JObject> storedEvent = _formatter.Serialize(sourcedEvent);
                        var bytes = storedEvent.GetBytes();
                        writer.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
                        writer.Write(bytes, 0, bytes.Length);
                        indicies[i++] = index;
                        index += bytes.Length;
                    }
                    UpdateEventSourceIndexFile(source.EventSourceId, indicies);
                    writer.Flush();
                }
            }
            finally
            {
                source.EventSourceId.ReleaseWriteLock();
            }
        }

        private void UpdateEventSourceIndexFile(Guid id, params long[] indicies)
        {
            var file = id.GetVersionFile(_path);
            var bytes = new byte[indicies.Length * 8];
            for (int i = 0; i < indicies.Length; i += 8)
            {
                var bytesIndex = i * 8;
                var intbytes = BitConverter.GetBytes(indicies[i]);

                for (int byteIndexOffset = 0; byteIndexOffset < 8; byteIndexOffset++)
                {
                    bytes[bytesIndex + byteIndexOffset] = intbytes[byteIndexOffset];
                }
            }
            using (var writer = file.OpenWrite())
            {
                writer.Seek(0, SeekOrigin.End);
                writer.Write(bytes, 0, 8);
            }
            
        }

        private long GetEventSourceIndexForVersion(Guid id, long version)
        {
            var file = id.GetVersionFile(_path);
            using (var reader = file.OpenRead())
            {
                reader.Seek(version*8, SeekOrigin.Begin);
                var indexBytes = new byte[8];
                reader.Read(indexBytes, 0, 8);
                return BitConverter.ToInt64(indexBytes, 0);
            }
        }

        private long GetVersion(Guid id)
        {
            var file = id.GetVersionFile(_path);
            return file.Length/8;
        }

        #endregion
    }
}
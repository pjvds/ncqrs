using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ncqrs.Eventing.Storage.Serialization;
using Newtonsoft.Json.Bson;
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

        public CommittedEventStream ReadFrom(Guid id, long minVersion, long maxVersion)
        {
            var events = new List<CommittedEvent>();
            FileInfo file = id.GetEventStoreFileInfo(_path);
            if (!file.Exists || GetVersion(id) <= minVersion)
            {
                return new CommittedEventStream(id);
            }
            var version = minVersion;
            if (version < 0)
            {
                version = 0;
            }
            try
            {
                id.GetReadLock();
                using (var reader = file.OpenRead())
                {
                    var indexBuf = new byte[4];
                    reader.Seek(GetEventSourceIndexForVersion(id, version), SeekOrigin.Begin);
                    var curVer = version + 1;
                    while ((reader.Read(indexBuf, 0, 4) == 4) && version < maxVersion)
                    {
                        var length = BitConverter.ToInt32(indexBuf, 0);
                        var eventBytes = new byte[length];
                        reader.Read(eventBytes, 0, length);
                        var evnt = ReadStoredEvent(eventBytes, id, curVer++);
                        events.Add(evnt);
                    }
                }
                return new CommittedEventStream(id, events);
            }
            finally
            {
                id.ReleaseReadLock();
            }
        }

        public void Store(UncommittedEventStream eventStream)
        {
            if (!eventStream.HasSingleSource)
            {
                throw new NotSupportedException("NoDBEventStore supports only one event source per Unit of Work.");
            }
            var sourceId = eventStream.SourceId;
            FileInfo file = sourceId.GetEventStoreFileInfo(_path);
            if (!file.Exists && !file.Directory.Exists)
                file.Directory.Create();
            try
            {
                sourceId.GetWriteLock();
                if (file.Exists)
                {
                    if (GetVersion(sourceId) >= eventStream.Sources.Single().InitialVersion)
                    {
                        throw new ConcurrencyException(sourceId, eventStream.Sources.Single().CurrentVersion);
                    }
                }
                using (var writer = file.OpenWrite())
                {
                    writer.Seek(0, SeekOrigin.End);
                    var indicies = new long[eventStream.Count()];
                    var i = 0;
                    var index = writer.Position;
                    foreach (var evnt in eventStream)
                    {
                        var bytes = GetBytes(evnt);
                        writer.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
                        writer.Write(bytes, 0, bytes.Length);
                        indicies[i++] = index;
                        index += bytes.Length;
                    }
                    UpdateEventSourceIndexFile(sourceId, indicies);
                    writer.Flush();
                }
            }
            finally
            {
                sourceId.ReleaseWriteLock();
            }
        }

        public byte[] GetBytes(UncommittedEvent evnt)
        {
            string eventName;
            JObject serializedPayload = _formatter.Serialize(evnt.Payload, out eventName);
            var output = new MemoryStream();
            var writer = new BinaryWriter(output);
            writer.Write(evnt.CommitId.ToByteArray());
            writer.Write(evnt.EventIdentifier.ToByteArray());
            writer.Write(evnt.EventSequence);
            writer.Write(evnt.EventTimeStamp.Ticks);
            writer.Write(eventName);
            writer.Write(evnt.EventVersion.ToString());
            var bsonWriter = new BsonWriter(output);
            serializedPayload.WriteTo(bsonWriter);
            bsonWriter.Flush();
            output.Flush();
            return output.ToArray();
        }

        public CommittedEvent ReadStoredEvent(byte[] eventBytes, Guid id, long version)
        {
            var input = new MemoryStream(eventBytes);
            var reader = new BinaryReader(input);
            var commitId = new Guid(reader.ReadBytes(16));
            var eventIdentifier = new Guid(reader.ReadBytes(16));
            var sequence = reader.ReadInt64();
            var timeStamp = new DateTime(reader.ReadInt64());
            var eventName = reader.ReadString();
            var eventVersion = new Version(reader.ReadString());
            var bsonReader = new BsonReader(input);
            var serializedPayload = JObject.Load(bsonReader);
            return new CommittedEvent(commitId, eventIdentifier, id, sequence, timeStamp, _formatter.Deserialize(serializedPayload, eventName), eventVersion);
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
                reader.Seek(version * 8, SeekOrigin.Begin);
                var indexBytes = new byte[8];
                reader.Read(indexBytes, 0, 8);
                return BitConverter.ToInt64(indexBytes, 0);
            }
        }

        private long GetVersion(Guid id)
        {
            var file = id.GetVersionFile(_path);
            return file.Length / 8;
        }

        #endregion
    }
}
using System;
using System.IO;
using System.Reflection;
using Ncqrs.Eventing.Sourcing.Snapshotting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ncqrs.Eventing.Storage.NoDB
{
    public class NoDBSnapshotStore : ISnapshotStore
    {
        private readonly string _path;

        public NoDBSnapshotStore(string path)
        {
            _path = path;
        }

        public Snapshot GetSnapshot(Guid eventSourceId, long maxVersion)
        {
            FileInfo file = eventSourceId.GetSnapshotFileInfo(_path);
            if (!file.Exists) return null;
            var reader = new StringReader(GetSnapshotText(eventSourceId, file.FullName));
            var typeline = reader.ReadLine();
            if (string.IsNullOrEmpty(typeline)) return null;
            var type = Type.GetType(typeline.Trim());
            try
            {
                var result = (Snapshot) new JsonSerializer().Deserialize(reader, type);
                return result.Version > maxVersion ? null : result;
            }
            catch(JsonSerializationException ex)
            {
                return null;
            }
        }

        public void SaveSnapshot(Snapshot source)
        {
            FileInfo file = source.EventSourceId.GetSnapshotFileInfo(_path);
            if (!file.Exists && !file.Directory.Exists)
                file.Directory.Create();
            var jo = JObject.FromObject(source);
            WriteSnapshotTest(source, file.FullName, jo.ToString());
        }

        private static void WriteSnapshotTest(Snapshot source, string path, string jsonData)
        {
            try
            {
                source.EventSourceId.GetWriteLock("snapshot");
                File.WriteAllText(path, source.GetType().AssemblyQualifiedName + "\n\r" + jsonData);
            } finally
            {
                source.EventSourceId.ReleaseWriteLock("snapshot");
            }
        }

        private static string GetSnapshotText(Guid eventSourceId, string path)
        {
            try
            {
                eventSourceId.GetReadLock("snapshot");
                var snapshottext = File.ReadAllText(path);
                return snapshottext;
            } finally
            {
                eventSourceId.ReleaseReadLock("snapshot");
            }
        }
    }
}
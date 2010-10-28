﻿using System;
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

        public void SaveShapshot(ISnapshot source)
        {
            FileInfo file = source.EventSourceId.GetSnapshotFileInfo(_path);
            if (!file.Exists && !file.Directory.Exists)
                file.Directory.Create();
            var jo = JObject.FromObject(source);
            WriteSnapshotTest(source, file.FullName, jo.ToString());
        }

        public ISnapshot GetSnapshot(Guid eventSourceId)
        {
            FileInfo file = eventSourceId.GetSnapshotFileInfo(_path);
            if (!file.Exists) return null;
            var reader = new StringReader(GetSnapshotText(eventSourceId, file.FullName));
            var typeline = reader.ReadLine();
            if (string.IsNullOrEmpty(typeline)) return null;
            var type = Type.GetType(typeline.Trim());
            try
            {
                return (ISnapshot) new JsonSerializer().Deserialize(reader, type);
            }
            catch(JsonSerializationException ex)
            {
                return null;
            }
        }

        private static void WriteSnapshotTest(ISnapshot source, string path, string jsonData)
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
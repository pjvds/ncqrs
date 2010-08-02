using System;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Ncqrs.Eventing.Storage.NoDB
{
    public static class StoredEventExtensions
    {
        public static string WriteLine(this StoredEvent<JObject> storedEvent)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0};{1};{2};{3};{4};",
                            storedEvent.EventIdentifier, storedEvent.EventTimeStamp.Ticks, storedEvent.EventName,
                            storedEvent.EventVersion, storedEvent.Data.ToString().Replace("\n", "").Replace("\r", ""));
            return sb.ToString();
        }

        public static StoredEvent<JObject> ReadStoredEvent(this string eventString, Guid id, long version)
        {
            string[] data = eventString.Split(';');
            return new StoredEvent<JObject>(new Guid(data[0]), new DateTime(long.Parse(data[1]),DateTimeKind.Utc), data[2],
                                            new Version(data[3]), id, version,
                                            JObject.Parse(data[4]));
        }

        public static FileInfo GetEventStoreFileInfo(this Guid eventSourceId, string rootPath)
        {
            return new FileInfo(GetPath(eventSourceId, rootPath));
        }

        public static FileInfo GetSnapshotFileInfo(this Guid eventSourceId, string rootPath)
        {
            return new FileInfo(GetPath(eventSourceId, rootPath) + ".ss");
        }

        private static string GetPath(Guid eventSourceId, string rootPath)
        {
            string foldername = eventSourceId.ToString().Substring(0, 2);
            string filename = eventSourceId.ToString().Substring(2);
            return Path.Combine(rootPath, foldername, filename);
        }
    }
}
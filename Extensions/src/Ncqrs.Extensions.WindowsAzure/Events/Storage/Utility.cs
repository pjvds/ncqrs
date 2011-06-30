using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure;
using Newtonsoft.Json;
using System.IO;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Sourcing.Snapshotting;
using System.Xml.Serialization;

namespace Ncqrs.Extensions.WindowsAzure.Events.Storage {
    public static class Utility {
        public static string GetRowKey(long value) {
            if (value < 0) {
                value = 0;
            }
            return value.ToString().PadLeft(long.MaxValue.ToString().Length + 1, '0');
        }
        private static IList<string> _tableNamesSetup = new List<string>();
        public static TableServiceContext GetContext(CloudStorageAccount account, string tableName) {
            
            TableServiceContext context = new TableServiceContext(account.TableEndpoint.AbsoluteUri, account.Credentials) {
                IgnoreMissingProperties = true,
                IgnoreResourceNotFoundException = true
            };
            // 628426 26 Jun 2011 - crazy malarky to allow development storage to support multiple entity types in the one table
            if (!_tableNamesSetup.Contains(tableName) && account == Microsoft.WindowsAzure.CloudStorageAccount.DevelopmentStorageAccount) {
                lock (_tableNamesSetup) {
                    if (!_tableNamesSetup.Contains(tableName)) {
                        EventEntity ee = new EventEntity();
                        ee.PartitionKey = Guid.NewGuid().ToString();
                        ee.RowKey = Guid.NewGuid().ToString();
                        ee.CommitId = Guid.NewGuid();
                        ee.EventIdentifier = Guid.NewGuid();
                        ee.EventSequence = 0;
                        ee.EventSourceId = Guid.NewGuid();
                        ee.EventTimeStamp = DateTime.Now;
                        ee.EventVersion = "EventVersion";
                        ee.Name = "Name";
                        ee.Payload = "Payload";
                        context.AddObject(tableName, ee);
                        context.SaveChanges();
                        context.DeleteObject(ee);
                        context.SaveChanges();

                        EventSourceEntity es = new EventSourceEntity();
                        es.PartitionKey = Guid.NewGuid().ToString();
                        es.RowKey = Guid.NewGuid().ToString();
                        es.Version = 0;
                        context.AddObject(tableName, es);
                        context.SaveChanges();
                        context.DeleteObject(es);
                        context.SaveChanges();

                        _tableNamesSetup.Add(tableName);
                    }
                }
            }

            return context;
        }
        public static string Jsonize(object data, Type type) {
            StringBuilder result = new StringBuilder();
            JsonSerializerSettings settings = new JsonSerializerSettings() {
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Full
            };
            
            JsonSerializer.Create(settings).Serialize(new StringWriter(result), data);
            return result.ToString();
        }

        public static string Jsonize(object data, string assemblyQualifiedTypeName) {
            Type parsedType = Type.GetType(assemblyQualifiedTypeName, true, true);
            return Jsonize(data, parsedType);
        }

        public static object DeJsonize(string data, Type type) {
            if (String.IsNullOrEmpty(data)) {
                return null;
            }
            JsonSerializerSettings settings = new JsonSerializerSettings() {
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Full
            };
            return JsonSerializer.Create(settings).Deserialize(new StringReader(data), type);
        }

        public static object DeJsonize(string data, string assemblyQualifiedTypeName) {
            if (String.IsNullOrEmpty(data)) {
                return null;
            }
            Type parsedType = Type.GetType(assemblyQualifiedTypeName, true, true);
            return DeJsonize(data, parsedType);
        }
        public static string GetSnapshotFullFileName(string container, Snapshot snapshot) {
            return GetSnapshotDirectoryName(container, snapshot.EventSourceId) + "/" + GetSnapshotFileName(snapshot.Version);
        }
        public static string GetSnapshotDirectoryName(string container, Guid eventSourceId) {
            return container + "/" + eventSourceId.ToString();
        }
        public static string GetSnapshotFileName(long version) {
            return GetRowKey(version) + ".ncqrssnapshot";
        }
        public static object DeXmlize(string data, string assemblyQualifiedTypeName) {
            return DeXmlize(data, Type.GetType(assemblyQualifiedTypeName, true, true));
        }

        public static string Xmlize(object data, string assemblyQualifiedTypeName) {
            return Xmlize(data, Type.GetType(assemblyQualifiedTypeName, true, true));
        }
        public static string Xmlize(object data, Type type) {
            string results = "";
            new XmlSerializer(type).Serialize(new StringWriter(new StringBuilder(results)), data);
            return results;
        }

        public static object DeXmlize(string data, Type type) {
            return new XmlSerializer(type).Deserialize(new StringReader(data));
        }
    }
}
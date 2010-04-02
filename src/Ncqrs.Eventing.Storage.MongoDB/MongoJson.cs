using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ncqrs.Eventing.Storage.MongoDB
{

    // TODO: Add real MongoDBNull
    public class MongoDBNull
    {
        public static string Value = null;
    }

    public class MongoJson
    {
        private const string _oidContainerName = "_id";

        public T ObjectFrom<T>(Document document)
            where T : class
        {
            if (document == null)
                return null;

            return JsonConvert.DeserializeObject<T>(document.ToString());
        }

        public Object ObjectFrom(Document document, Type objectType)
        {
            if (document == null)
                return null;

            return JsonConvert.DeserializeObject(document.ToString(), objectType);
        }

        public Document DocumentFrom(string json)
        {
            return PopulateDocumentFrom(new Document(), json);
        }

        public Document DocumentFrom<T>(T item)
            where T : class
        {
            return PopulateDocumentFrom(new Document(), item);
        }

        public Document PopulateDocumentFrom<T>(Document document, T item)
            where T : class
        {
            if (item == null)
                return document;

            var json = JsonConvert.SerializeObject(item, Formatting.None);

            return PopulateDocumentFrom(document, json);
        }

        private Document PopulateDocumentFrom(Document document, string json)
        {
            var keyValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            foreach (var keyValue in keyValues)
            {
                var isEmptyKeyField = (keyValue.Key == _oidContainerName && document[_oidContainerName] != MongoDBNull.Value);

                if (isEmptyKeyField)
                    continue;

                var value = keyValue.Value ?? MongoDBNull.Value;

                if (value != MongoDBNull.Value)
                {
                    var arrayValue = (keyValue.Value as JArray);
                    if (arrayValue != null)
                        value = arrayValue.Select(j => (string)j).ToArray();
                }

                if (document.Contains(keyValue.Key))
                    document[keyValue.Key] = value;
                else
                {
                    if (value != MongoDBNull.Value)
                        document.Add(keyValue.Key, value);
                }
            }

            return document;
        }
    }

}

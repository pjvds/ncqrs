using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace Ncqrs.Eventing.Storage.WindowsAzure
{
    public static class Utility
    {
        public static string Jsonize(object data, Type type)
        {
            StringBuilder result = new StringBuilder();
            NcqrsEnvironment.Get<JsonSerializer>().Serialize(new StringWriter(result), data);
            return result.ToString();
        }

        public static string Jsonize(object data, string assemblyQualifiedTypeName)
        {
            Type parsedType = Type.GetType(assemblyQualifiedTypeName, true, true);
            return Jsonize(data, parsedType);
        }

        public static object DeJsonize(string data, Type type)
        {
            return NcqrsEnvironment.Get<JsonSerializer>().Deserialize(new StringReader(data), type);
        }

        public static object DeJsonize(string data, string assemblyQualifiedTypeName)
        {
            Type parsedType = Type.GetType(assemblyQualifiedTypeName, true, true);
            return DeJsonize(data, parsedType);
        }

    }
}

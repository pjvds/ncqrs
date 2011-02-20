using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Ncqrs.Eventing.Storage.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ncqrs.Eventing.Storage.WindowsAzure 
{
    public class Utility 
    {
        public static object DeserializeFrom(byte[] bytes)
        {
            return new BinaryFormatter().Deserialize(new MemoryStream(bytes));
        }

        public static byte[] SerializeTo(object payload)
        {
            using (MemoryStream output = new MemoryStream())
            {
                new BinaryFormatter().Serialize(output, payload);
                return output.ToArray();
            }
        }

        public static string JsonizeTo(object payload)
        {
            using(StringWriter sw = new StringWriter())
            {
                new JsonSerializer().Serialize(sw, payload);
                return sw.ToString();
            }
        }

        public static object JsonizeFrom(string json, Type type)
        {
            return new JsonSerializer().Deserialize(new StringReader(json), type);
        }

        public static object JsonizeFrom(string json, string typeName)
        {
            Type returnType = Type.GetType(typeName, true, false);
            return JsonizeFrom(json, returnType );
        }


    }
}

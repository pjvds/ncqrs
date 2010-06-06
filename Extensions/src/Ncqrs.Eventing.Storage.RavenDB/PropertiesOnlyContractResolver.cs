using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace Ncqrs.Eventing.Storage.RavenDB
{
    public class PropertiesOnlyContractResolver : DefaultContractResolver
    {
        public PropertiesOnlyContractResolver()
        {
            DefaultMembersSearchFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        }

        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            var result = base.GetSerializableMembers(objectType);
            return result.Where(x => x.MemberType == MemberTypes.Property).ToList();
        }
    }
}
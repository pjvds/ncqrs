using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{
    internal static class SnapshotableField
    {
        public static IEnumerable<FieldInfo> GetAll(Type type)
        {
            while (type != null)
            {
                if (type.HasAttribute<DynamicSnapshotAttribute>())
                    foreach (var field in GetSnapshotableFields(type))
                        yield return field;

                type = type.BaseType;
            }
        }

        public static IDictionary<string, FieldInfo> GetMap(Type type)
        {
            var map = new Dictionary<string, FieldInfo>();
            var fields = GetAll(type);

            foreach (var field in fields)
            {
                string key = GenerateFieldKey(field);
                map.Add(key, field);
            }

            return map;
        }

        private static string GenerateFieldKey(FieldInfo field)
        {
            string key = string.Format("{0}{1}", field.DeclaringType.FullName, field.Name);
            return key;
        }

        private static IEnumerable<FieldInfo> GetSnapshotableFields(Type sourceType)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
            return sourceType.GetFields(flags).Where(field => IsSnapshotable(field));
        }

        private static bool IsSnapshotable(FieldInfo field)
        {
            return field.GetCustomAttributes(typeof(ExcludeFromSnapshotAttribute), false).Count() == 0;
        }
    }
}
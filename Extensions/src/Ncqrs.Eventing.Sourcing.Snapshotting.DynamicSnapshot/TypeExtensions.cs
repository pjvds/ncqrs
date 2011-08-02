using System;
using System.Linq;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{
    public static class TypeExtensions
    {
        public static bool HasAttribute(this Type type, Type attributeType, bool inherit = false)
        {
            return type.GetCustomAttributes(attributeType, inherit).Count() > 0;
        }

        public static bool HasAttribute<TAttribute>(this Type type, bool inherit = false) where TAttribute : Attribute
        {
            return type.HasAttribute(typeof(TAttribute), inherit);
        }
    }
}

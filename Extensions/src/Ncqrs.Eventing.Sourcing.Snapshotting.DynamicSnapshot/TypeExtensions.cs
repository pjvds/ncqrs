using System;
using System.Linq;

namespace System.Linq
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

        public static bool Inherits(this Type type, Type baseType)
        {
            return baseType.IsAssignableFrom(type);
        }

        public static bool Inherits<T>(this Type type) where T : class
        {
            return type.Inherits(typeof(T));
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ncqrs
{
    internal static class InternalExtensions
    {
        public static bool IsEmpty(this IEnumerable target)
        {
            return (target.Cast<object>().Count() == 0);
        }

        public static bool IsEmpty<TSource>(this IEnumerable<TSource> target)
        {
            return (target.Count() == 0);
        }

        public static bool IsNullOrEmpty(this string target)
        {
            return String.IsNullOrEmpty(target);
        }

        public static List<T> Clone<T>(this List<T> target)
        {
            return new List<T>(target);
        }

        public static bool IsNullable(this Type type)
        {
            return !type.IsValueType || (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)));
        }

    }
}

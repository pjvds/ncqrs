using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ncqrs
{
    internal static class InternalExtensions
    {
        public static String FormatWith(this String target, params object[] args)
        {
            return String.Format(target, args);
        }

        public static bool Empty(this IEnumerable target)
        {
            return (target.Cast<object>().Count() == 0);
        }

        public static bool Empty<TSource>(this IEnumerable<TSource> target)
        {
            return (target.Count() == 0);
        }
    }
}

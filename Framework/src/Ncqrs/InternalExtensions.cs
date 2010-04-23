using System;

namespace Ncqrs
{
    internal static class InternalExtensions
    {
        public static String FormatWith(this String target, params object[] args)
        {
            return String.Format(target, args);
        }
    }
}

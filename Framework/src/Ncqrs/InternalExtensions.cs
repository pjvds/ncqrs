using System;
using System.Collections.Generic;
using System.Linq;

namespace Ncqrs
{
    /// <summary>
    /// Provides general extensions that are used inside the framework code.
    /// </summary>
    internal static class InternalExtensions
    {
        /// <summary>
        /// Determine whether an IEnumerable source is empty or not.
        /// </summary>
        /// <typeparam name="TSource">The type of the objects that can be enumerated.</typeparam>
        /// <param name="source">The source to check. This may not be null.</param>
        /// <exception cref="ArgumentNullException">Occurs when <paramref name="source"/> is <c>null</c>.</exception>
        /// <returns><c>true</c> whenever the source did not contain any items to enumerate; otherwise, <c>false</c>.</returns>
        public static bool IsEmpty<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            return (source.Count() == 0);
        }

        public static bool Implements<TInterfaceType>(this Type source)
        {
            Type interfaceType = typeof (TInterfaceType);
            return Implements(source, interfaceType);
        }

        public static bool Implements(this Type source, Type interfaceType)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (interfaceType == null) throw new ArgumentNullException("interfaceType");
            
            if (!interfaceType.IsInterface)
                throw new ArgumentException("The provided interface type is not an interface.", "interfaceType");

            return interfaceType.IsAssignableFrom(source);
        }

        /// <summary>
        /// Determines whether a string is <c>null</c> or <see cref="String.Empty"/>
        /// </summary>
        /// <param name="source">The source to check.</param>
        /// <returns>
        /// 	<c>true</c> if <paramref name="source"/> is <c>null</c> or <see cref="String.Empty"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty(this string source)
        {
            return String.IsNullOrEmpty(source);
        }

        /// <summary>
        /// Clones a list. It returns a new list with the same content.
        /// </summary>
        /// <typeparam name="T">The type of the objects in the list.</typeparam>
        /// <param name="source">The source to clone. This cannot be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">Occurs when <paramref name="source"/> is <c>null</c>.</exception>
        /// <returns>A new instance that contains the same elements in the same order as the source.</returns>
        public static List<T> Clone<T>(this List<T> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            return new List<T>(source);
        }

        /// <summary>
        /// Determines whether the specified type is nullable.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// 	<c>true</c> if the specified type is nullable; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullable(this Type type)
        {
            return !type.IsValueType || (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)));
        }
    }
}

using System;

namespace Ncqrs.Commands.AutoMapping
{
    /// <summary>
    /// Defines that the command maps directly to a constructor on an aggregate root.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class MapsToAggregateRootConstructorAttribute : Attribute
    {
        /// <summary>
        /// Get or sets the full qualified type name of the target aggregate root.
        /// </summary>
        public String TypeName
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapsToAggregateRootConstructorAttribute"/> class.
        /// </summary>
        /// <param name="typeName">The full qualified name of the type of the aggregate root.</param>
        /// <exception cref="ArgumentNullException">Thrown when <i>typeName</i> is null or emtpy.</exception>
        public MapsToAggregateRootConstructorAttribute(String typeName)
        {
            if(String.IsNullOrEmpty(typeName)) throw new ArgumentNullException(typeName);

            TypeName = typeName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapsToAggregateRootConstructorAttribute"/> class.
        /// </summary>
        /// <param name="type">The type of the aggregate root.</param>
        /// <exception cref="ArgumentNullException">Thrown when <i>type</i> is null.</exception>
        public MapsToAggregateRootConstructorAttribute(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            TypeName = type.FullName;
        }
    }
}

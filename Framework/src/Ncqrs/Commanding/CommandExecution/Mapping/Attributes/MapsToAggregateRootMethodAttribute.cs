using System;
using System.Linq;
using System.Reflection;
using Ncqrs.Domain;

namespace Ncqrs.Commanding.CommandExecution.Mapping.Attributes
{
    /// <summary>
    /// Defines that the command maps directly to a method on an aggregate root.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class MapsToAggregateRootMethodAttribute : Attribute
    {
        private Type _type;

        /// <summary>
        /// Get or sets the full qualified type name of the target aggregate root.
        /// </summary>
        public String TypeName
        {
            get;
            private set;
        }

        /// <summary>
        /// Get the type of the target aggregate root.
        /// </summary>
        public Type Type
        {
            get
            {
                //delay resolving the type from type name to avoid potentially
                //loading another assembly whilst holding class loader locks.
                if (_type == null)
                    _type = Type.GetType(TypeName, true);
                return _type;
            }
        }

        /// <summary>
        /// Get or sets the full qualified name of the target method. Leave this null or empty to automap the target method based on the command name.
        /// </summary>
        public String MethodName
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapsToAggregateRootMethodAttribute"/> class.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        public MapsToAggregateRootMethodAttribute(String typeName)
        {
            TypeName = typeName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapsToAggregateRootMethodAttribute"/> class.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="methodName">Name of the method.</param>
        public MapsToAggregateRootMethodAttribute(string typeName, string methodName)
        {
            if (String.IsNullOrEmpty(typeName)) throw new ArgumentNullException("typeName");
            if (String.IsNullOrEmpty(methodName)) throw new ArgumentNullException("methodName");

            TypeName = typeName;
            MethodName = methodName;
        }

        public MapsToAggregateRootMethodAttribute(Type type, String methodName)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (String.IsNullOrEmpty(methodName)) throw new ArgumentNullException("methodName");

            _type = type;
            TypeName = type.AssemblyQualifiedName;
            MethodName = methodName;
        }

    }
}

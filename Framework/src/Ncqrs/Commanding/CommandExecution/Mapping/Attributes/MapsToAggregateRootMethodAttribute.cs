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
    public class MapsToAggregateRootMethodAttribute : CommandMappingAttribute
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

        public override ICommandExecutor<TCommand> CreateExecutor<TCommand>()
        {
            var prvt = BindingFlags.NonPublic | BindingFlags.Instance;
            var methodInfo = GetType().GetMethod("CreateExecutor", prvt, null,
                                             Type.EmptyTypes, null);

            var genericMethod = methodInfo.MakeGenericMethod(typeof (TCommand), Type);
            return (ICommandExecutor<TCommand>)genericMethod.Invoke(this, null);
        }

        private ICommandExecutor<TCommand> CreateExecutor<TCommand, TAggregateRoot>() where TCommand : ICommand where TAggregateRoot : AggregateRoot
        {
            var commandType = typeof (TCommand);
            ValidateCommandType(commandType);

            var match = GetMatchingMethod(commandType, MethodName);

            return new DirectActionCommandExecutor<TCommand, TAggregateRoot>(
                GetAggregateRootId,
                (agg, cmd) =>
                    {
                        var parameter = match.Item2.Select(p => p.GetValue(cmd, null));
                        match.Item1.Invoke(agg, parameter.ToArray());
                    }
                );
        }

        private Tuple<MethodInfo, PropertyInfo[]> GetMatchingMethod(Type commandType, string methodName)
        {
            var strategy = new AttributePropertyMappingStrategy();
            var sources = strategy.GetMappedProperties(commandType);

            return PropertiesToMethodMapper.GetMethod(sources, Type, methodName);
        }

        private Guid GetAggregateRootId<TCommand>(TCommand cmd)
        {
            var all = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            var commandType = typeof (TCommand);
            var idProp = commandType.GetProperties(all).Single(p => p.IsDefined(typeof (AggregateRootIdAttribute), false));
            return (Guid)idProp.GetValue(cmd, null);
        }

        private void ValidateCommandType(Type mappedCommandType)
        {
            bool containsThisAttribute = mappedCommandType.IsDefined(GetType(), false);

            if (!containsThisAttribute) throw new ArgumentException("The given command type does not contain " +
                                                                    "MapsToAggregateRootConstructorAttribute.",
                                                                    "mappedCommandType");
        }
    }
}

using System;
using System.Linq;
using System.Reflection;

namespace Ncqrs.Commanding.CommandExecution.Mapping.Attributes
{
    /// <summary>
    /// Defines that the command maps directly to a constructor on an aggregate root.
    /// </summary>
    public class MapsToAggregateRootConstructorAttribute : CommandMappingAttribute
    {
        private Type _type;

        /// <summary>
        /// Get the full qualified type name of the target aggregate root.
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

            _type = type;
            TypeName = type.AssemblyQualifiedName;
        }

        public override ICommandExecutor<TCommand> CreateExecutor<TCommand>()
        {
            var commandType = typeof (TCommand);
            ValidateCommandType(commandType);

            var match = GetMatchingConstructor(commandType);
            return new AggregateRootCreationCommandExecutor<TCommand>((c) =>
            {
                var parameter = match.Item2.Select(p => p.GetValue(c, null));
                match.Item1.Invoke(parameter.ToArray());
            });
        }

        private Tuple<ConstructorInfo, PropertyInfo[]> GetMatchingConstructor(Type commandType)
        {
            return ClassToMethodMapper.GetConstructor(commandType, Type);
        }

        private void ValidateCommandType(Type mappedCommandType)
        {
            bool containsThisAttribute = mappedCommandType.IsDefined(GetType(), false);

            if(!containsThisAttribute) throw new ArgumentException("The given command type does not contain "+
                                                                   "MapsToAggregateRootConstructorAttribute.",
                                                                   "mappedCommandType");
        }
    }
}

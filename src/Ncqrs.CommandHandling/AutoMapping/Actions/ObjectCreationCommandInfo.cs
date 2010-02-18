using System;
using Ncqrs.Commands;
using Ncqrs.Commands.AutoMapping;
using System.Reflection;

namespace Ncqrs.CommandHandling.AutoMapping.Actions
{
    internal class ObjectCreationCommandInfo
    {
        /// <summary>
        /// Gets the type of the aggregate root.
        /// </summary>
        /// <value>The type of the aggregate root.</value>
        public Type AggregateType
        {
            get;
            private set;
        }

        public ICommand Command
        {
            get; private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectCreationCommandInfo"/> struct.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="aggregateType">Type of the aggregate.</param>
        public ObjectCreationCommandInfo(ICommand command, Type aggregateType)
        {
            Command = command;
            AggregateType = aggregateType;
        }

        public static ObjectCreationCommandInfo CreateFromDirectMethodCommand(ICommand command)
        {
            if(command == null) throw new ArgumentNullException("command");
            var aggregateRootType = GetAggregateRootType(command);

            return new ObjectCreationCommandInfo(command, aggregateRootType);
        }

        private static Type GetAggregateRootType(ICommand command)
        {
            var mappingAttribute = GetMappingAttribute(command);
            // TODO: Add exception wrapping.

            return Type.GetType(mappingAttribute.TypeName, true);
        }

        private static MapsToAggregateRootConstructorAttribute GetMappingAttribute(ICommand command)
        {
            var type = command.GetType();
            var mappingAttributes = (MapsToAggregateRootConstructorAttribute[])type.GetCustomAttributes(typeof(MapsToAggregateRootConstructorAttribute), true);

            if (mappingAttributes.Length == 0)
            {
                String message = String.Format("Missing MapsToAggregateRootConstructorAttribute on {0} command.", type.Name);
                throw new CommandMappingException(message);
            }
            if (mappingAttributes.Length > 1)
            {
                String message = String.Format("Multiple MapsToAggregateRootConstructorAttribute found on {0} command, only one attribute is allowed.", type.Name);
                throw new CommandMappingException(message);
            }

            return mappingAttributes[0];
        }
    }
}
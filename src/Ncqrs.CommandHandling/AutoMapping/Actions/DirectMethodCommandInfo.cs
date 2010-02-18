using System;
using System.Linq;
using System.Reflection;
using Ncqrs.Commands;
using Ncqrs.Commands.AutoMapping;

namespace Ncqrs.CommandHandling.AutoMapping.Actions
{
    internal class DirectMethodCommandInfo
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

        /// <summary>
        /// Gets the aggregate root id.
        /// </summary>
        /// <value>The id of the aggregate root.</value>
        public Guid AggregateRootIdValue
        {
            get;
            private set;
        }

        public ICommand Command
        {
            get; private set;
        }

        public String MethodName
        {
            get; private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectMethodCommandInfo"/> struct.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="aggregateType">Type of the aggregate.</param>
        /// <param name="aggregateRootIdValue">The aggregate root id.</param>
        public DirectMethodCommandInfo(ICommand command, Type aggregateType, Guid aggregateRootIdValue, String methodName)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (aggregateType == null) throw new ArgumentNullException("aggregateType");
            if (String.IsNullOrEmpty(methodName)) throw new ArgumentNullException("methodName");

            Command = command;
            AggregateType = aggregateType;
            AggregateRootIdValue = aggregateRootIdValue;
            MethodName = methodName;
        }

        public static DirectMethodCommandInfo CreateFromDirectMethodCommand(ICommand command)
        {
            if(command == null) throw new ArgumentNullException("command");
            var aggregateRootType = GetAggregateRootType(command);
            var aggregateRootId = GetAggregateRootId(command);
            var attribute = GetMappingAttribute(command);
            var methodName = String.IsNullOrEmpty(attribute.MethodName) ? command.GetType().Name : attribute.MethodName;
            return new DirectMethodCommandInfo(command, aggregateRootType, aggregateRootId, methodName);
        }

        private static Type GetAggregateRootType(ICommand command)
        {
            var mappingAttribute = GetMappingAttribute(command);

            try
            {
                return Type.GetType(mappingAttribute.TypeName, true);
            }
            catch (Exception e)
            {
                var message = String.Format("Couldn't determ aggregate root type from typename '{0}'.", mappingAttribute.TypeName);
                throw new AutoMappingException(message, e);
            }
        }

        private static Guid GetAggregateRootId(ICommand command)
        {
            var prop = GetPropertyMarkedAsAggregateRootId(command);

            if(prop.PropertyType != typeof(Guid))
            {
                String message = String.Format("Property {0} that marked as aggregate root id is not of type Guid.", prop.Name);
                throw new CommandMappingException(message);
            }

            return (Guid)prop.GetValue(command, null);
        }

        private static PropertyInfo GetPropertyMarkedAsAggregateRootId(ICommand command)
        {
            var type = command.GetType();
            var propertyQuery = from prop in type.GetProperties()
                                where prop.GetCustomAttributes(typeof(AggregateRootIdAttribute), true).Count() > 0
                                select prop;

            if (propertyQuery.Count() == 0)
            {
                String message = String.Format("Missing AggregateRootIdAttribute on {0} command.", type.Name);
                throw new CommandMappingException(message);
            }
            if(propertyQuery.Count() > 1)
            {
                String message = String.Format("Multiple AggregateRootIdAttribute found on {0} command, only one attribute is allowed.", type.Name);
                throw new CommandMappingException(message);
            }

            return propertyQuery.First();
        }

        private static MapsToAggregateRootMethodAttribute GetMappingAttribute(ICommand command)
        {
            var type = command.GetType();
            var mappingAttributes = (MapsToAggregateRootMethodAttribute[])type.GetCustomAttributes(typeof(MapsToAggregateRootMethodAttribute), true);

            if (mappingAttributes.Length == 0)
            {
                String message = String.Format("Missing MapsToAggregateRootMethodAttribute on {0} command.", type.Name);
                throw new CommandMappingException(message);
            }
            if (mappingAttributes.Length > 1)
            {
                String message = String.Format("Multiple MapsToAggregateRootMethodAttribute found on {0} command, only one attribute is allowed.", type.Name);
                throw new CommandMappingException(message);
            }

            return mappingAttributes[0];
        }
    }
}
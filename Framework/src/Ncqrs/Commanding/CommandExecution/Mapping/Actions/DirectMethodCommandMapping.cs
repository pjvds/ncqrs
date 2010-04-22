using System;
using System.Linq;
using System.Reflection;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;
using System.Diagnostics.Contracts;

namespace Ncqrs.Commanding.CommandExecution.Mapping.Actions
{
    public interface IDirectMethodCommandMapping
    {
        /// <summary>
        /// Gets the type of the aggregate root.
        /// </summary>
        /// <value>The type of the aggregate root that contains the method.</value>
        Type AggregateRootType
        { 
            get;
        }

        /// <summary>
        /// Gets the aggregate root id value.
        /// </summary>
        /// <value>The aggregate root id value.</value>
        Guid AggregateRootIdValue
        { 
            get;
        }

        /// <summary>
        /// Gets the name of the method.
        /// </summary>
        /// <value>The name of the method.</value>
        String MethodName
        { 
            get;
        }
    }

    internal class AttributeBasedDirectMethodCommandMapping : IDirectMethodCommandMapping
    {
        /// <summary>Gets or sets the type of the aggregate root. </summary>
        /// <value> The type of the aggregate root. </value>
        public Type AggregateRootType
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
        /// Initializes a new instance of the <see cref="AttributeBasedDirectMethodCommandMapping"/>.
        /// </summary>
        /// <param name="command">The command.</param>
        public AttributeBasedDirectMethodCommandMapping(ICommand command)
        {
            Contract.Requires<ArgumentNullException>(command != null, "The command cannot be null.");

            Command = command;
            AggregateRootType = GetAggregateRootType(command);
            AggregateRootIdValue = GetAggregateRootId(command);
            MethodName = GetMappingAttribute(command).MethodName;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;

namespace Ncqrs.Commanding.CommandExecution.Mapping.Attributes
{
    public class AttributeBasedCommandMapper : ICommandMapper
    {
        private readonly Dictionary<Type, object> _handlers = new Dictionary<Type, object>();

        public AttributeBasedCommandMapper()
        {
            RegisterAttributeHandler(new MapsToAggregateRootConstructorAttributeHandler());
            RegisterAttributeHandler(new MapsToAggregateRootMethodAttributeHandler());
        }

        public void RegisterAttributeHandler<T>(IMappingAttributeHandler<T> handler)
        {
            _handlers[typeof (T)] = handler;
        }

        /// <summary>
        /// Determines whether the type is a mapped command.
        /// </summary>
        /// <param name="type">The type to check. This value cannot be null.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is <c>null</c>.
        /// </exception>
        /// <returns>
        /// 	<c>true</c> if [is command mapped] [the specified target]; otherwise, <c>false</c>.
        /// </returns>
        public bool CanMapCommand(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            return type.Implements<ICommand>() &&
                   IsAttributeHandlerRegistered(type);
        }

        private bool IsAttributeHandlerRegistered(Type type)
        {
            return type.GetCustomAttributes(false).Any(x => _handlers.ContainsKey(x.GetType()));
        }

        private static Attribute GetCommandMappingAttributeFromType(Type target)
        {
            var attributes = (Attribute[])target.GetCustomAttributes(typeof(Attribute), false);

            if (attributes.Length == 0)
            {
                var msg = string.Format("Could not create executor for command {0} based on " +
                    "attribute mapping. It does not contain a MapsToAggregateRootConstructorAttribute " +
                    "or MapsToAggregateRootMethodAttribute.", target.FullName);

                throw new CommandMappingException(msg);
            }

            if (attributes.Length > 1)
            {
                var msg = string.Format("Could not create executor for command {0} based on " +
                    "attribute mapping. It does contain multiple mapping attributes.",
                    target.FullName);

                throw new CommandMappingException(msg);
            }

            return attributes.Single();
        }

        public void Map(ICommand command, IMappedCommandExecutor executor)
        {
            var commandType = command.GetType();
            dynamic mappingAttr = GetCommandMappingAttributeFromType(commandType);

            dynamic attributeHandler;
            if (!_handlers.TryGetValue(mappingAttr.GetType(), out attributeHandler))
            {
                throw new CommandMappingException(string.Format("Coulnd not find attribute handler for mapping attribute of type {0}.", mappingAttr.GetType().AssemblyQualifiedName));
            }
            attributeHandler.Map(mappingAttr, command, executor);
        }
    }
}
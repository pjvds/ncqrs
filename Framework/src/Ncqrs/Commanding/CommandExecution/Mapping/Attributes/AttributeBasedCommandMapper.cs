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

        public void Map(ICommand command, IMappedCommandExecutor executor)
        {
            var commandType = command.GetType();
            IEnumerable<dynamic> attributes = commandType.GetCustomAttributes(false);

            dynamic attributeHandler;

            foreach (dynamic attribute in attributes)
            {
                if (_handlers.TryGetValue(attribute.GetType(), out attributeHandler))
                {
                    attributeHandler.Map(attribute, command, executor);
                    return;
                }
            }
            throw new CommandMappingException(string.Format("Coulnd not find any mapping attribute handlers for mapping command of type {0}.", command.GetType().AssemblyQualifiedName));
        }
    }
}
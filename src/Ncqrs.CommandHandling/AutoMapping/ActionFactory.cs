using System;
using Ncqrs.CommandHandling.AutoMapping.Actions;
using Ncqrs.Commands;
using Ncqrs.Commands.Attributes;
using System.Diagnostics.Contracts;
using Ncqrs.Domain.Storage;

namespace Ncqrs.CommandHandling.AutoMapping
{
    /// <summary>
    /// A factory to use the create action for commands based on mapping.
    /// </summary>
    public class ActionFactory
    {
        private readonly IDomainRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionFactory"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <exception cref="ArgumentNullException">Occurs when <i>repository</i> is <c>null</c>.</exception>
        public ActionFactory(IDomainRepository repository)
        {
            Contract.Requires<ArgumentNullException>(repository != null, "The repository cannot be null.");

            _repository = repository;
        }

        /// <summary>
        /// Creates an action for command based on mapping.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <exception cref="ActumentNullException">Occurs when <i>command</i> was <c>null</c>.</exception>
        /// <exception cref="MappingForCommandNotFoundException">Occurs when there was an error in the mapping of the command.</exception>
        /// <returns>A <see cref="IAutoMappedCommandAction"/> action created based on the mapping of the command.</returns>
        public IAutoMappedCommandAction CreateActionForCommand(ICommand command)
        {
            Contract.Requires<ArgumentNullException>(command != null);

            ValidateCommand(command);

            if (IsCommandMappedToObjectCreation(command))
            {
                return new ObjectCreationAction(_repository, command);
            }

            if (IsCommandMappedToADirectMethod(command))
            {
                return new DirectMethodAction(_repository, command);
            }

            var message = String.Format("No mapping attributes found on {0} command.", command.GetType().Name);
            throw new MappingForCommandNotFoundException(message, command);
        }

        private void ValidateCommand(ICommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
        }

        private static Boolean IsCommandMappedToADirectMethod(ICommand command)
        {
            return IsAttributeDefinedOnCommand<MapsToAggregateRootMethodAttribute>(command);
        }

        private static Boolean IsCommandMappedToObjectCreation(ICommand command)
        {
            return IsAttributeDefinedOnCommand<MapsToAggregateRootConstructorAttribute>(command);
        }

        private static Boolean IsAttributeDefinedOnCommand<T>(ICommand command)
        {
            var type = command.GetType();
            var attributes = type.GetCustomAttributes(false);

            foreach(var attrib in attributes)
            {
                if(attrib is T)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
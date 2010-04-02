using System;
using Ncqrs.CommandHandling.AutoMapping.Actions;
using Ncqrs.Commands;
using Ncqrs.Commands.Attributes;
using System.Diagnostics.Contracts;
using Ncqrs.Domain.Storage;

namespace Ncqrs.CommandHandling.AutoMapping
{
    public class ActionFactory
    {
        private readonly IDomainRepository _repository;

        public ActionFactory(IDomainRepository repository)
        {
            Contract.Requires(repository != null);

            _repository = repository;
        }

        public IAutoMappedCommandAction CreateActionForCommand(ICommand command) // TODO: Is a command always mapped to only one action?
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

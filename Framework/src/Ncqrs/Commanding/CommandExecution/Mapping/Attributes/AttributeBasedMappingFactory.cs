using System;
using System.Linq;

namespace Ncqrs.Commanding.CommandExecution.Mapping.Attributes
{
    public class AttributeBasedMappingFactory
    {
        public ICommandExecutor<TCommand> CreateMappingForCommand<TCommand>() where TCommand : ICommand
        {
            var commandType = typeof(TCommand);
            return (ICommandExecutor<TCommand>)CreateMappingForCommand(commandType);
        }

        public ICommandExecutor<ICommand> CreateMappingForCommand(Type commandType)
        {
            var mappingAttr = GetCommandMappingAttributeFromType(commandType);

            return mappingAttr.CreateExecutor(commandType);
        }

        public bool IsCommandMapped(Type target)
        {
            return target.IsDefined(typeof (CommandMappingAttribute), false);
        }

        private CommandMappingAttribute GetCommandMappingAttributeFromType(Type target)
        {
            var attributes = (CommandMappingAttribute[]) target.GetCustomAttributes(typeof (CommandMappingAttribute), false);

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
    }
}
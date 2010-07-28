using System;
using System.Linq;
using System.Diagnostics.Contracts;

namespace Ncqrs.Commanding.CommandExecution.Mapping.Attributes
{
    public class AttributeBasedMappingFactory
    {
        public ICommandExecutor<TCommand> CreateExecutorForCommand<TCommand>() where TCommand : ICommand
        {
            Contract.Ensures(Contract.Result <ICommandExecutor<TCommand>>() != null);

            var commandType = typeof(TCommand);
            var mappingAttr = GetCommandMappingAttributeFromType(commandType);
            return mappingAttr.CreateExecutor<TCommand>();
        }

        public ICommandExecutor<ICommand> CreateExecutorForCommand(Type commandType)
        {
            var mappingAttr = GetCommandMappingAttributeFromType(commandType);

            var method = mappingAttr.GetType().GetMethod("CreateExecutor", Type.EmptyTypes);
            var genericMethod = method.MakeGenericMethod(commandType);

            object executor = genericMethod.Invoke(mappingAttr, null);
            var executeMethod = executor.GetType().GetMethod("Execute");

            Action<ICommand> redirection = (cmd) => executeMethod.Invoke(executor, new object[] { cmd });

            return new CommandExecutorWrapper<ICommand>(redirection);
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
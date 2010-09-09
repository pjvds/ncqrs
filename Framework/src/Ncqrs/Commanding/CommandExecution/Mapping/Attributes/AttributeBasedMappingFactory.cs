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
            ICommandExecutor<TCommand> executor = null;

            try
            {
                executor = mappingAttr.CreateExecutor<TCommand>();
            }
            catch (Exception exception)
            {
                var msg = string.Format("Couldn't executor for command {0} based on mapping.", commandType.FullName);
                throw new CommandMappingException(msg, exception);
            }

            return executor;
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

        /// <summary>
        /// Determines whether the type is a mapped command.
        /// </summary>
        /// <remarks>
        /// A type is a mapped command when it implements the <see cref="ICommand"/> interface
        /// and is marked with an attribute that inhires the <see cref="CommandMappingAttribute"/>.
        /// </remarks>
        /// <param name="type">The type to check. This value cannot be null.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is <c>null</c>.
        /// </exception>
        /// <returns>
        /// 	<c>true</c> if [is command mapped] [the specified target]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCommandMapped(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            return type.Implements<ICommand>() &&
                   type.IsDefined(typeof(CommandMappingAttribute), false);
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
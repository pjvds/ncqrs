using System;
using System.Diagnostics.Contracts;
using Ncqrs.Commanding.CommandExecution.Mapping.Reflection;

namespace Ncqrs.Commanding.CommandExecution.Mapping
{
    /// <summary>
    /// A factory to use the create action for commands based on mapping.
    /// </summary>
    public class ActionFactory
    {
        /// <summary>
        /// Creates an executor for command based on mapping.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <exception cref="ArgumentNullException">Occurs when <i>command</i> was <c>null</c>.</exception>
        /// <exception cref="MappingForCommandNotFoundException">Occurs when there was an error in the mapping of the command.</exception>
        /// <returns>A <see cref="ICommandExecutor"/> created based on the mapping of the command.</returns>
        public ICommandExecutor<TCommand> CreateExecutorForCommand<TCommand>(TCommand command) where TCommand : ICommand
        {
            Contract.Requires<ArgumentNullException>(command != null);

            if (IsCommandMappedToObjectCreation(command.GetType()))
            {
                return new ObjectCreationCommandExecutor<TCommand>();
            }

            if (IsCommandMappedToADirectMethod(command.GetType()))
            {
                return new DirectMethodCommandExecutor<TCommand>();
            }

            var message = String.Format("No mapping attributes found on {0} command.", command.GetType().Name);
            throw new MappingForCommandNotFoundException(message, command);
        }

        /// <summary>
        /// Determines whether the commands is mapped.
        /// </summary>
        /// <param name="commandType">Type of the command</param>
        /// <returns>True, if command is mapped. False otherwise.</returns>
        public bool IsCommandMapped(Type commandType)
        {
            return
                IsCommandMappedToObjectCreation(commandType) ||
                IsCommandMappedToADirectMethod(commandType);
        }

        /// <summary>
        /// Determines whether the command is mapped to a direct method.
        /// </summary>
        /// <param name="commandType">Type of the command.</param>
        /// <returns>
        /// 	<c>true</c> if the command is mapped to a direct method; otherwise, <c>false</c>.
        /// </returns>
        private static Boolean IsCommandMappedToADirectMethod(Type commandType)
        {
            return commandType.IsDefined(typeof (MapsToAggregateRootMethodAttribute), false);
        }

        /// <summary>
        /// Determines whether the command is mapped for object creation.
        /// </summary>
        /// <param name="commandType">Type of the command.</param>
        /// <returns>
        /// 	<c>true</c> if the is command mapped for object creation; otherwise, <c>false</c>.
        /// </returns>
        private static Boolean IsCommandMappedToObjectCreation(Type commandType)
        {
            return commandType.IsDefined(typeof (MapsToAggregateRootConstructorAttribute), false);
        }        
    }
}
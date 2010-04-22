using System;
using System.Diagnostics.Contracts;
using Ncqrs.Commanding.CommandExecution.Mapping.Actions;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;

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
        public ICommandExecutor CreateExecutorForCommand(ICommand command)
        {
            Contract.Requires<ArgumentNullException>(command != null);

            if (IsCommandMappedToObjectCreation(command))
            {
                return new ObjectCreationAction();
            }

            if (IsCommandMappedToADirectMethod(command))
            {
                return new DirectMethodAction();
            }

            var message = String.Format("No mapping attributes found on {0} command.", command.GetType().Name);
            throw new MappingForCommandNotFoundException(message, command);
        }

        /// <summary>
        /// Determines whether the command is mapped to a direct method.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>
        /// 	<c>true</c> if the command is mapped to a direct method; otherwise, <c>false</c>.
        /// </returns>
        private static Boolean IsCommandMappedToADirectMethod(ICommand command)
        {
            return IsAttributeDefinedOnCommand<MapsToAggregateRootMethodAttribute>(command);
        }

        /// <summary>
        /// Determines whether the command is mapped for object creation.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>
        /// 	<c>true</c> if the is command mapped for object creation; otherwise, <c>false</c>.
        /// </returns>
        private static Boolean IsCommandMappedToObjectCreation(ICommand command)
        {
            return IsAttributeDefinedOnCommand<MapsToAggregateRootConstructorAttribute>(command);
        }

        /// <summary>
        /// Determines whether the specified attribute is defined on the command.
        /// </summary>
        /// <typeparam name="T">The type of the attribute.</typeparam>
        /// <param name="command">The command.</param>
        /// <returns>
        /// 	<c>true</c> if the specified attribute is defined on the given command; otherwise, <c>false</c>.
        /// </returns>
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
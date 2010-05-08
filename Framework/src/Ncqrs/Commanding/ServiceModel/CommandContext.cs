using System;
using Ncqrs.Commanding.CommandExecution;

namespace Ncqrs.Commanding.ServiceModel
{
    /// <summary>Represents the context of a command execution in an 
    /// <see cref="ICommandService"/>. This context is passed through 
    /// <see cref="ICommandServiceInterceptor">interceptors</see> for each
    /// interception.</summary>
    public class CommandContext
    {
        /// <summary>
        /// <para>Gets the exception that has occurred while handling the event.
        /// Use the <see cref="IsExecuted"></see> property to determine whether
        /// the exception has been thrown by a interceptor or by the execution
        /// itself.</para>
        /// </summary>
        /// <value>The exception that has been occurred or <c>null</c> when it
        /// has not.</value>
        public Exception Exception
        {
            get; internal set;
        }

        /// <summary>
        /// Gets the command that will be or is executed.
        /// </summary>
        /// <value>This value is never <c>null</c>.</value>
        public ICommand TheCommand
        {
            get; internal set;
        }

        /// <summary>
        /// Gets the command executor that will be or is used to execute the
        /// command.
        /// </summary>
        /// <value>When this value is <c>null</c>, it means that there was no
        /// executor found for the command.</value>
        public ICommandExecutor TheCommandExecutor
        {
            get; internal set;
        }

        /// <summary>
        /// Gets a value indicating whether the command is executed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the command is executed; otherwise, <c>false</c>.
        /// <remarks>
        /// This value is always <c>false</c> in the <see cref="ICommandServiceInterceptor.OnBeforeExecution"/>.
        /// </remarks>
        /// </value>
        public bool IsExecuted
        {
            get; internal set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandContext"/> class.
        /// </summary>
        /// <param name="theCommand">The command that will be executed.</param>
        public CommandContext(ICommand theCommand)
        {
            TheCommand = theCommand;
        }
    }
}

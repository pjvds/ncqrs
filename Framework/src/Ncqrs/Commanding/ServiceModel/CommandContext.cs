using System;
using Ncqrs.Commanding.CommandExecution;
using System.Diagnostics.Contracts;

namespace Ncqrs.Commanding.ServiceModel
{
    /// <summary>Represents the context of a command execution in an 
    /// <see cref="ICommandService"/>. This context is passed through 
    /// <see cref="ICommandServiceInterceptor">interceptors</see> for each
    /// interception.</summary>
    public class CommandContext
    {
        private ICommand _theCommand;

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
            get;
            internal set;
        }

        public Boolean ExecutorResolved
        {
            get; internal set;
        }

        /// <summary>
        /// Gets the command that will be or is executed.
        /// </summary>
        /// <exception cref="InvalidOperationException">Occurs when this
        /// property is set while the <see cref="TheCommandExecutor"/> property
        /// has already been set.</exception>
        /// <value>This value is never <c>null</c>.</value>
        public ICommand TheCommand
        {
            get
            {
                Contract.Ensures(Contract.Result<ICommand>() != null, "The result cannot be null.");

                return _theCommand;
            }
            set
            {
                Contract.Requires<InvalidOperationException>(!ExecutorResolved, "Cannot override command when command executor is already resolved.");
                _theCommand = value;
            }
        }

        /// <summary>
        /// Gets the type of the command.
        /// </summary>
        /// <value>The type of the command.</value>
        public Type TheCommandType
        {
            get
            {
                return TheCommand.GetType();
            }
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
            get;
            internal set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandContext"/> class.
        /// </summary>
        /// <param name="theCommand">The command that will be executed.</param>
        public CommandContext(ICommand theCommand)
        {
            Contract.Requires<ArgumentNullException>(theCommand != null, "The theCommand cannot be null.");

            _theCommand = theCommand;
        }
    }
}

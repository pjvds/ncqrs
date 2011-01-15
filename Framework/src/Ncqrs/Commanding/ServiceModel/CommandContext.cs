using System;
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
        /// Use the <see cref="ExecutorHasBeenCalled"></see> property to determine whether
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

        /// <summary>
        /// Gets a value indicating whether an executor has been resolved.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if an executor has been resolved for this command; otherwise, <c>false</c>.
        /// </value>
        public Boolean ExecutorResolved
        {
            get; internal set;
        }

        /// <summary>
        /// Gets the command that will be or is executed.
        /// </summary>
        /// <exception cref="InvalidOperationException">Occurs when this
        /// property is set while the <see cref="ExecutorResolved"/> property
        /// return true indicating that a executor has been resolved for the command.</exception>
        /// <value>The command that will or has been executed. This value is never <c>null</c>.</value>
        public ICommand TheCommand
        {
            get
            {
                Contract.Ensures(Contract.Result<ICommand>() != null, "The result cannot be null.");

                return _theCommand;
            }
            set
            {
                Contract.Requires<InvalidOperationException>(!ExecutorResolved, "Cannot override command when a command executor has already been resolved.");
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
        /// Gets a value indicating whether the executor for the command has
        /// been called.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the executor for this command has been called;
        /// 	otherwise, <c>false</c>. <c>true</c> does not have to mean that the
        /// 	executor also executed the command. It could be that an exception
        /// 	had occurred.
        /// <remarks>
        /// This value is always <c>false</c> in the 
        /// <see cref="ICommandServiceInterceptor.OnBeforeExecution"/>.
        /// </remarks>
        /// </value>
        public bool ExecutorHasBeenCalled
        {
            get;
            internal set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandContext"/> class.
        /// </summary>
        /// <param name="theCommand">The command that will be executed.</param>
        /// <param name="state">The current state of the command context</param>
        /// <param name="exception">Any exception which may have been thrown while executing</param>
        public CommandContext(ICommand theCommand, CommandExecutionState state = CommandExecutionState.None, Exception exception = null)
        {
            Contract.Requires<ArgumentNullException>(theCommand != null, "The theCommand cannot be null.");

            _theCommand = theCommand;
            Exception = exception;
            if (state == CommandExecutionState.Resolved) ExecutorResolved = true;
            if (state == CommandExecutionState.Called)
            {
                ExecutorResolved = true;
                ExecutorHasBeenCalled = true;
            }
        }
    }

    public enum CommandExecutionState { None, Resolved, Called }
}

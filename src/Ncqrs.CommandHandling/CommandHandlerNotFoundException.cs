using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace Ncqrs.CommandHandling
{
    /// <summary>
    /// Occurs when a handler was not found to execute the command.
    /// </summary>
    [Serializable]
    public class CommandHandlerNotFoundException : Exception
    {
        /// <summary>
        /// Gets the type of the command.
        /// </summary>
        public Type CommandType
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandlerNotFoundException"/> class.
        /// </summary>
        /// <param name="commandType">Type of the command.</param>
        /// <exception cref="ArgumentNullException">Occurs when <i>commandType</i> is a <c>null</c> dereference.</exception>
        public CommandHandlerNotFoundException(Type commandType) : this(commandType, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandlerNotFoundException"/> class.
        /// </summary>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="ArgumentNullException">Occurs when <i>commandType</i> is a <c>null</c> dereference.</exception>
        public CommandHandlerNotFoundException(Type commandType, string message) : this(commandType, message, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandlerNotFoundException"/> class.
        /// </summary>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        /// <exception cref="ArgumentNullException">Occurs when <i>commandType</i> is a <c>null</c> dereference.</exception>
        public CommandHandlerNotFoundException(Type commandType, string message, Exception inner) : base((String.IsNullOrEmpty(message) ? String.Format("No handler was found for command {0}.", commandType.GetType().FullName) : message), inner)
        {
            Contract.Requires<ArgumentNullException>(commandType != null);

            CommandType = commandType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandlerNotFoundException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected CommandHandlerNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

        [ContractInvariantMethod]
        protected virtual void ContractInvariant()
        {
            Contract.Invariant(CommandType != null);
        }
    }
}

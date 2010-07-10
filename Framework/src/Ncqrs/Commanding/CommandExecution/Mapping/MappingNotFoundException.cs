using System;
using System.Runtime.Serialization;

namespace Ncqrs.Commanding.CommandExecution.Mapping
{
    /// <summary>
    /// Occurs when there is no auto mapping found for a <see cref="ICommand"/>.
    /// </summary>
    [Serializable]
    public class MappingNotFoundException : CommandMappingException
    {
        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <value>The command.</value>
        public ICommand Command
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="command">The command.</param>
        public MappingNotFoundException(string message, ICommand command)
            : this(message, command, null)
        {
        }

        public MappingNotFoundException(string message, ICommand command, Exception inner) : base(message, inner)
        {
            Command = command;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingNotFoundException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown. </param><param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination. </param><exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception><exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected MappingNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

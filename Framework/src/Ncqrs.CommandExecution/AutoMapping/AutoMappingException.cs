using System;
using System.Runtime.Serialization;

namespace Ncqrs.CommandExecution.AutoMapping
{
    /// <summary>
    /// Occurs when an command could not be mapped.
    /// </summary>
    [Serializable]
    public class AutoMappingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMappingException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the exception.</param>
        public AutoMappingException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMappingException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the exception.</param>
        /// <param name="innerException">The inner exception.</param>
        public AutoMappingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMappingException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown. </param><param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination. </param><exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception><exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected AutoMappingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

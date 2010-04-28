using System;
using System.Runtime.Serialization;

namespace Ncqrs.Domain
{
    /// <summary>
    /// Thrown when a <see cref="IUnitOfWork"/> is requested but was not available in the context.
    /// </summary>
    [Serializable]
    public class NoUnitOfWorkAvailableInThisContextException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoUnitOfWorkAvailableInThisContextException"/> class.
        /// </summary>
        public NoUnitOfWorkAvailableInThisContextException() : this("There is no unit of work available in this context.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoUnitOfWorkAvailableInThisContextException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public NoUnitOfWorkAvailableInThisContextException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoUnitOfWorkAvailableInThisContextException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public NoUnitOfWorkAvailableInThisContextException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoUnitOfWorkAvailableInThisContextException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        protected NoUnitOfWorkAvailableInThisContextException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}

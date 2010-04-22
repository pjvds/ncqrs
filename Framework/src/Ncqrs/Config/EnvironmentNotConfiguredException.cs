using System;
using System.Runtime.Serialization;

namespace Ncqrs.Config
{
    /// <summary>
    /// Thrown when the <see cref="NcqrsEnvironment"/> is used when it is not configured.
    /// </summary>
    [Serializable]
    public class EnvironmentNotConfiguredException : NcqrsEnvironmentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentNotConfiguredException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public EnvironmentNotConfiguredException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentNotConfiguredException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public EnvironmentNotConfiguredException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentNotConfiguredException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected EnvironmentNotConfiguredException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}

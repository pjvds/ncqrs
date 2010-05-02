using System;
using System.Runtime.Serialization;

namespace Ncqrs.Config
{
    /// <summary>
    /// Occurs when a requested instance could not be found in the Ncqrs environment configuration.
    /// </summary>
    [Serializable]
    public class InstanceNotFoundInEnvironmentConfigurationException : NcqrsEnvironmentException
    {
        /// <summary>
        /// Gets the type of the requested instance.
        /// </summary>
        /// <value>The type of the requested instance.</value>
        public Type RequestedType
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceNotFoundInEnvironmentConfigurationException"/> class.
        /// </summary>
        /// <param name="requestedType">Type of the requested instance.</param>
        public InstanceNotFoundInEnvironmentConfigurationException(Type requestedType)
            : this(requestedType, requestedType != null ? BuildDefaultMessage(requestedType) : null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceNotFoundInEnvironmentConfigurationException"/> class.
        /// </summary>
        /// <param name="requestedType">Type of the requested instance.</param>
        /// <param name="message">The message.</param>
        public InstanceNotFoundInEnvironmentConfigurationException(Type requestedType, string message)
            : this(requestedType, message, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceNotFoundInEnvironmentConfigurationException"/> class.
        /// </summary>
        /// <param name="requestedType">Type of the requested instance.</param>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public InstanceNotFoundInEnvironmentConfigurationException(Type requestedType, string message, Exception inner)
            : base(message, inner)
        {
            RequestedType = requestedType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceNotFoundInEnvironmentConfigurationException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected InstanceNotFoundInEnvironmentConfigurationException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }

        private static string BuildDefaultMessage(Type requestedType)
        {
            return String.Format("Could not find requested type {0} in the NcqrsEnvironment configuration. Make sure that " +
                                 "the NcqrsEnvironment is configured correctly or that defaults are correctly set.", requestedType.FullName);
        }
    }
}

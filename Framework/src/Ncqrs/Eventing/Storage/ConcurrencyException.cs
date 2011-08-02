using System;
using System.Runtime.Serialization;

namespace Ncqrs.Eventing.Storage
{
    /// <summary>
    /// Occurs when there is already a newer version of the event source stored in the event store.
    /// </summary>
    [Serializable]
    public class ConcurrencyException : Exception
    {
        private readonly Guid _eventSourceId;
        private readonly long _eventSourceVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrencyException"/> class.
        /// </summary>
        /// <param name="eventSourceId">The id of the event source.</param>
        /// <param name="versionToBeSaved">Version to be saved.</param>
        public ConcurrencyException(Guid eventSourceId, long versionToBeSaved)
            : base(String.Format("There is a newer than {0} version of the event source with id {1} you are trying to save stored in the event store.", versionToBeSaved, eventSourceId))
        {
            _eventSourceId = eventSourceId;
            _eventSourceVersion = versionToBeSaved;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrencyException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected ConcurrencyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _eventSourceId = (Guid) info.GetValue("EventSourceId", typeof (Guid));
            _eventSourceVersion = info.GetInt64("EventSourceVersion");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("EventSourceId", _eventSourceId);
            info.AddValue("EventSourceVersion", _eventSourceVersion);
        }

        /// <summary>
        /// Gets the id of the event source.
        /// </summary>
        /// <value>The id event source.</value>
        public Guid EventSourceId
        {
            get { return _eventSourceId; }
        }


        /// <summary>
        /// Gets the event source version.
        /// </summary>
        /// <value>The event source version.</value>
        public long EventSourceVersion
        {
            get { return _eventSourceVersion; }
        }

        
    }
}

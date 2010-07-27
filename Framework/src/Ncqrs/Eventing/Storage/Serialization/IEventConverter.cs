using System;
using System.Diagnostics.Contracts;
using Newtonsoft.Json.Linq;

namespace Ncqrs.Eventing.Storage.Serialization
{
    /// <summary>
    /// Converts old versions of events to the latest version of that event.
    /// </summary>
    /// <seealso cref="EventConverter"/>
    /// <seealso cref="NullEventConverter"/>
    [ContractClass(typeof(IEventConverterContracts))]
    public interface IEventConverter
    {
        /// <summary>
        /// Upgrades an event to the latest version.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is responsible for determining if the event needs
        /// upgrading, and if so upgrading it to the latest version.
        /// </para>
        /// 
        /// <para>
        /// Upgrading the event is handled by altering the structure and the
        /// data in the <see cref="JObject"/> stored in the
        /// <see cref="StoredEvent{T}.Data"/> property.
        /// </para>
        /// 
        /// <para>
        /// If it upgrades the event to a newer version it must update the
        /// <see cref="StoredEvent{T}.EventVersion"/> property.
        /// </para>
        /// </remarks>
        /// <param name="theEvent">The event to be upgraded.</param>
        void Upgrade(StoredEvent<JObject> theEvent);
    }

    [ContractClassFor(typeof(IEventConverter))]
    internal abstract class IEventConverterContracts : IEventConverter
    {
        public void Upgrade(StoredEvent<JObject> theEvent)
        {
            Contract.Requires<ArgumentNullException>(theEvent != null, "theEvent");
        }
    }
}

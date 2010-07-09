using Newtonsoft.Json.Linq;

namespace Ncqrs.Eventing.Storage.Serialization
{
    /// <summary>
    /// Converts old versions of events to the latest version of that event.
    /// </summary>
    /// <seealso cref="EventConverter"/>
    /// <seealso cref="NullEventConverter"/>
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
}

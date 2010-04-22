using System;

namespace Ncqrs.Domain.Storage
{
    /// <summary>
    /// Occurs when there is already a newer version of the event provider stored in the event store.
    /// </summary>
    public class ConcurrencyException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrencyException"/> class.
        /// </summary>
        /// <param name="providerVersion">The provider version.</param>
        /// <param name="versionInStore">The version in store.</param>
        public ConcurrencyException(long providerVersion, long versionInStore)
        {
            ProviderVersion = providerVersion;
            VersionInStore = versionInStore;
        }

        /// <summary>
        /// Gets the provider version.
        /// </summary>
        /// <value>The provider version.</value>
        public long ProviderVersion { get; private set; }

        /// <summary>
        /// Gets the version of the provider in the event store.
        /// </summary>
        /// <value>The version in store.</value>
        public long VersionInStore { get; private set; }
    }
}

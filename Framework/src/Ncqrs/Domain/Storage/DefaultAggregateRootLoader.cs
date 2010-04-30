using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ncqrs.Domain.Storage
{
    /// <summary>
    /// The default aggregate root loader than can load an aggregate root instance from a historic event stream.
    /// </summary>
    public class DefaultAggregateRootLoader : IAggregateRootLoader
    {
        /// <summary>
        /// Loads the aggregate root from historical events.
        /// </summary>
        /// <remarks>
        /// This method searches for a public or non public constructor that accepts no parameters and invokes it to create the instance. After
        /// creating the instance the <see cref="AggregateRoot.InitializeFromHistory"/> method is called.
        /// </remarks>
        /// <param name="aggregateRootType">Type of the aggregate root to load.</param>
        /// <param name="events">The historical events.</param>
        /// <returns>
        /// A new instance of the specified aggregate root type loaded with context that has been build from the events.
        /// </returns>
        /// <exception cref="AggregateLoaderException">Occurs when the aggregate root does not contains a constructor that
        /// accepts no parameters.</exception>
        public AggregateRoot LoadAggregateRootFromEvents(Type aggregateRootType, IEnumerable<DomainEvent> events)
        {
            // Flags to search for a public and non public contructor.
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            // Get the constructor that we want to invoke.
            var ctor = aggregateRootType.GetConstructor(flags, null, Type.EmptyTypes, null);

            // If there was no ctor found, throw exception.
            if (ctor == null)
            {
                var message = String.Format("No constructor found on aggregate root type {0} that accepts " +
                                            "no parameters.", aggregateRootType.AssemblyQualifiedName);
                throw new AggregateLoaderException(message);
            }

            // There was a ctor found, so invoke it and return the instance.
            var aggregateRoot = (AggregateRoot)ctor.Invoke(null);

            // Initialize the aggregate root from the history.
            aggregateRoot.InitializeFromHistory(events);

            return aggregateRoot;
        }
    }
}
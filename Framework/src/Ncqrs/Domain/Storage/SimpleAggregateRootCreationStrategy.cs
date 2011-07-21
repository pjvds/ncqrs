using System;
using System.Reflection;

namespace Ncqrs.Domain.Storage
{
    public class SimpleAggregateRootCreationStrategy 
        : AggregateRootCreationStrategy
    {

        protected override AggregateRoot CreateAggregateRootFromType(Type aggregateRootType)
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
                throw new AggregateRootCreationException(message);
            }

            // There was a ctor found, so invoke it and return the instance.
            var aggregateRoot = (AggregateRoot)ctor.Invoke(null);

            return aggregateRoot;
        }

    }
}

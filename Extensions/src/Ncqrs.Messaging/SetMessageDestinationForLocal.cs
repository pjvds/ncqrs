using System;
using Ncqrs.Domain;

namespace Ncqrs.Messaging
{
    public static class SetMessageDestinationForLocal
    {
        public static ISetMessageRequirements Aggregate<T>(this ISetMessageDestination @this, Guid id)
            where T : AggregateRoot
        {
            return @this.NamedEndpoint(LocalInMemoryReceivingStrategy.MakeId(typeof (T), id));
        }
    }
}
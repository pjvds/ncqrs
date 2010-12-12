using System;
using Ncqrs.Domain;

namespace Ncqrs.Messaging
{
    public static class SetInternalMessageDestination
    {
        private static readonly IAddressing _addressing = new UrlAddressing();

        public static ISetMessageRequirements Aggregate<T>(this ISetMessageDestination @this, Guid id)
            where T : AggregateRoot
        {
            return @this.Endpoint(_addressing.EncodeAddress(new Destination(typeof(T), id)));
        }
    }
}
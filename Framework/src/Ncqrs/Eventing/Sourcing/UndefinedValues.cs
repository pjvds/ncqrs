using System;
using Ncqrs.Eventing.Storage;

namespace Ncqrs.Eventing.Sourcing
{
    public class UndefinedValues
    {
        public static Guid UndefinedEventSourceId = Guid.Empty;
        public const int UndefinedEventSequence = -1;
    }
}

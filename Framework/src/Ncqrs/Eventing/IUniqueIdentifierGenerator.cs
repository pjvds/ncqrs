using System;

namespace Ncqrs.Eventing
{
    public interface IUniqueIdentifierGenerator
    {
        Guid GenerateNewId(EventSource eventSource);
    }
}

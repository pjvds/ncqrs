using System;

namespace Ncqrs.Eventing
{
    public interface ISnapshot
    {
        Guid EventSourceId
        {
            get;
        }

        long EventSourceVersion
        {
            get;
        }
    }
}

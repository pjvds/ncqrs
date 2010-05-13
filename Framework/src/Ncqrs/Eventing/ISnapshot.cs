using System;

namespace Ncqrs.Eventing
{
    public interface ISnapshot
    {
        IMemento Memento
        {
            get;
        }

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

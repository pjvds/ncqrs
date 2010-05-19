using System;

namespace Ncqrs.Eventing
{
    public interface IMemento
    {
        long ForVersion
        {
            get;
        }
    }
}

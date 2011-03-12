using System;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{
    public interface IHaveProxyReference
    {
        object Proxy { set; }
    }
}

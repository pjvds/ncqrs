using System;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{
    /// <summary>
    /// If you can kill me please do it. I want to die.
    /// </summary>
    public interface IHaveProxyReference
    {
        object Proxy { set; }
    }
}

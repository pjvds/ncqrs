using System;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{
    /// <summary>
    /// If you can kill me please do it. I want to die.
    /// After all, I think I'm pretty immortal right now...
    /// </summary>
    public interface IHaveProxyReference
    {
        object Proxy { set; }
    }
}

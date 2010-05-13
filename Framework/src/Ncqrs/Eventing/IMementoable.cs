using System;
using Ncqrs.Domain;

namespace Ncqrs.Eventing
{
    /// <summary>
    /// This interface flags an object beeing <i>mementoable</i>. This means that the
    /// state of the object could be saved to an <see cref="IMemento"/> object
    /// and restored from a the from the same class.
    /// This is used to prevent building <see cref="AggregateRoot"/>'s from the ground up.
    /// </summary>
    public interface IMementoable<TMemento> where TMemento : IMemento
    {
        void RestoreFromMemento(TMemento memento);
        TMemento CreateMemento();
    }
}

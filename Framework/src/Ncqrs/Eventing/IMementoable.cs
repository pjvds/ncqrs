using System;
using Ncqrs.Domain;
using System.Diagnostics.Contracts;

namespace Ncqrs.Eventing
{
    /// <summary>
    /// This interface flags an object beeing <i>mementoable</i>. This means that the
    /// state of the object could be saved to an <see cref="IMemento"/> object
    /// and restored from a the from the same class.
    /// This is used to prevent building <see cref="AggregateRoot"/>'s from the ground up.
    /// </summary>
    [ContractClass(typeof(IMementoableContracts<>))]
    public interface IMementoable<TMemento> where TMemento : IMemento
    {
        void RestoreFromMemento(TMemento memento);
        TMemento CreateMemento();
    }

    [ContractClassFor(typeof(IMementoable<>))]
    public class IMementoableContracts<TMemento> : IMementoable<TMemento> where TMemento : IMemento
    {
        public void RestoreFromMemento(TMemento memento)
        {
        }

        public TMemento CreateMemento()
        {
            return default(TMemento);
        }
    }
}

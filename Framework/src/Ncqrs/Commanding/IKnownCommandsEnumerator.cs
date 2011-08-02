using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Ncqrs.Commanding
{
    /// <summary>
    /// 
    /// </summary>
    [ContractClass(typeof(IKnownCommandsEnumeratorContracts))]
    public interface IKnownCommandsEnumerator
    {
        IEnumerable<Type> GetAllCommandTypes();
    }

    [ContractClassFor(typeof(IKnownCommandsEnumerator))]
    internal abstract class IKnownCommandsEnumeratorContracts : IKnownCommandsEnumerator
    {
        public IEnumerable<Type> GetAllCommandTypes()
        {
            Contract.Ensures(Contract.Result<IEnumerable<Type>>() != null);
            return default(IEnumerable<Type>);
        }
    }
}

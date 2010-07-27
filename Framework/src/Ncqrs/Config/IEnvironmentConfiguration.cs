using System;
using System.Diagnostics.Contracts;

namespace Ncqrs.Config
{
    /// <summary>
    /// A configuration that can resolve requested instances.
    /// </summary>
    [ContractClass(typeof(IEnvironmentConfigurationContracts))]
    public interface IEnvironmentConfiguration
    {
        /// <summary>
        /// Tries to get the specified instance.
        /// </summary>
        /// <typeparam name="T">The type of the instance to get.</typeparam>
        /// <param name="result">The result.</param>
        /// <returns>A indication whether the instance could be get or not.</returns>
        Boolean TryGet<T>(out T result) where T : class;
    }

    [ContractClassFor(typeof(IEnvironmentConfiguration))]
    internal abstract class IEnvironmentConfigurationContracts : IEnvironmentConfiguration
    {
        public T Get<T>() where T : class
        {
            Contract.Ensures(Contract.Result<T>() != null);

            return default(T);
        }

        public bool TryGet<T>(out T result) where T : class
        {
            Contract.Ensures(Contract.Result<bool>() ? Contract.ValueAtReturn(out result) != null : Contract.ValueAtReturn(out result) == null);

            result = default(T);
            return true;
        }
    }
}
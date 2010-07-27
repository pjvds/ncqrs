using System;
using System.Diagnostics.Contracts;

namespace Ncqrs
{
    /// <summary>
    /// Represents a clock that can give the current time.
    /// </summary>
    [ContractClass(typeof(IClockContracts))]
    public interface IClock
    {
        /// <summary>
        /// Gets the current UTC date and time from the clock.
        /// </summary>
        /// <returns>The current UTC date and time.</returns>
        DateTime UtcNow();
    }

    [ContractClassFor(typeof(IClock))]
    internal abstract class IClockContracts : IClock
    {
        public DateTime UtcNow()
        {
            Contract.Ensures(Contract.Result<DateTime>().Kind == DateTimeKind.Utc, "The result should be a UTC date and time.");
            return default(DateTime);
        }
    }
}
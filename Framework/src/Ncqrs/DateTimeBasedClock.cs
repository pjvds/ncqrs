using System;

namespace Ncqrs
{
    /// <summary>
    /// A clock that is based on the <see cref="DateTime"/> class from the .NET framework.
    /// </summary>
    public class DateTimeBasedClock : IClock
    {
        /// <summary>
        /// Gets the current UTC date and time from the clock.
        /// </summary>
        /// <returns>The current UTC date and time.</returns>
        public DateTime UtcNow()
        {
            return DateTime.UtcNow;
        }
    }
}

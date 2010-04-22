using System;
using System.Diagnostics.Contracts;

namespace Ncqrs.Eventing
{
    public class HistoricalEventAppliedEventArgs : EventArgs
    {
        public HistoricalEvent HistoricalEvent
        {
            get;
            private set;
        }

        public HistoricalEventAppliedEventArgs(HistoricalEvent historicalEvent)
        {
            Contract.Requires<ArgumentNullException>(historicalEvent != null);
            Contract.Ensures(HistoricalEvent == historicalEvent);

            HistoricalEvent = historicalEvent;
        }
    }
}

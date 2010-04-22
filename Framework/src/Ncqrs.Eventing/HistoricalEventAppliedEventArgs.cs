using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace Ncqrs.Domain
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

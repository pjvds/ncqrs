using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Ncqrs.Eventing
{
    public class SourcedEventStream : IEnumerable<ISourcedEvent>
    {
        private Guid _eventSourceId;
        private long _sequence;

        public SourcedEventStream(Guid eventSourceId, long sequence)
        {
            _eventSourceId = eventSourceId;
            _sequence = sequence;
        }

        [ContractInvariantMethod]
        protected void ContractInvariants()
        {
            Contract.Invariant(_eventSourceId != Guid.Empty);
            Contract.Invariant(_sequence >= 0);
        }

        public IEnumerator<ISourcedEvent> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

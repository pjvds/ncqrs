using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Eventing.Storage.NoDB
{
    public class NoDBEventStore : IEventStore
    {
        private readonly string _path;

        public NoDBEventStore(string path)
        {
            _path = path;
        }

        public IEnumerable<SourcedEvent> GetAllEvents(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SourcedEvent> GetAllEventsSinceVersion(Guid id, long version)
        {
            throw new NotImplementedException();
        }

        public void Save(IEventSource source)
        {
            throw new NotImplementedException();
        }
    }
}

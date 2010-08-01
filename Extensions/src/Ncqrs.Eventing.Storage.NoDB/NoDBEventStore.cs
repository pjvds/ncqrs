using System;
using System.Collections.Generic;
using System.IO;
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
            var foldername = source.EventSourceId.ToString().Substring(0, 2);
            var filename = source.EventSourceId.ToString().Substring(2);
            if (!Directory.Exists(Path.Combine(_path, foldername)))
                Directory.CreateDirectory(Path.Combine(_path, foldername));
            File.Create(Path.Combine(_path, foldername, filename));
        }
    }
}

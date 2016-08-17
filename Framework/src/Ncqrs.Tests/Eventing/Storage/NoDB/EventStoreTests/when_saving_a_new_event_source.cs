using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Ncqrs.Eventing.Storage.NoDB.Tests.EventStoreTests
{
    public class when_saving_a_new_event_source : NoDBEventStoreTestFixture
    {
        private string _filename;
        private string _foldername;

        public when_saving_a_new_event_source() : base()
        {
            //Bandaid for file system race. See issue #54 for full explanation
            Thread.Sleep(TimeSpan.FromSeconds(1));

            _foldername = GetPath();
            _filename = EventSourceId.ToString().Substring(2);
        }

        [Fact(Skip= "File system race. Also, NoDBEventStore serialization is incompatible with StoredEventExtensions. See issue #54 for full explanation.")]
        public void it_should_create_a_new_event_history_file()
        {
            Assert.True(File.Exists(Path.Combine(_foldername, _filename)));
        }

        [Fact(Skip = "File system race. Also, NoDBEventStore serialization is incompatible with StoredEventExtensions. See issue #54 for full explanation.")]
        public void it_should_have_at_least_one_event()
        {
            using (var reader = new StreamReader(File.Open(Path.Combine(_foldername, _filename), FileMode.Open)))
            {
                reader.ReadLine(); //Throw out version line
                Assert.True(GetEventStrings(reader).Any(), "We read the file before the event store wrote the event.");
            }
        }

        [Fact(Skip = "File system race. Also, NoDBEventStore serialization is incompatible with StoredEventExtensions. See issue #54 for full explanation.")]
        public void it_should_serialize_the_uncommitted_events_to_the_file()
        {
            using (var reader = new StreamReader(File.Open(Path.Combine(_foldername, _filename), FileMode.Open)))
            {
                reader.ReadLine(); //Throw out version line
                int i = 0;
                foreach (string line in GetEventStrings(reader))
                {
                    Console.WriteLine(line);
                    StoredEvent<JObject> storedevent = line.ReadStoredEvent(EventSourceId, i);
                    i++;
                    Assert.NotNull(storedevent);
                }
            }
        }

        private static IEnumerable<string> GetEventStrings(TextReader reader)
        {
            string line = reader.ReadLine();
            while (line != null)
            {
                yield return line;
                line = reader.ReadLine();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Ncqrs.Eventing.Storage.NoDB.Tests.EventStoreTests
{
    [TestFixture]
    [Ignore("Tests failing when executed in CMD (e.q. running BUILD.bat), they succeed when executed in Visual Studio.")]
    public class when_saving_a_new_event_source : NoDBEventStoreTestFixture
    {
        private string _filename;
        private string _foldername;

        [TestFixtureSetUp]
        public void SetUp()
        {
            BaseSetup();

            _foldername = Source.EventSourceId.ToString().Substring(0, 2);
            _filename = Source.EventSourceId.ToString().Substring(2);
        }

        [Test]
        public void it_should_create_a_new_event_history_file()
        {
            Assert.That(File.Exists(Path.Combine(_foldername, _filename)));
        }

        [Test]
        public void it_should_serialize_the_uncommitted_events_to_the_file()
        {
            using (var reader = new StreamReader(File.Open(Path.Combine(_foldername, _filename), FileMode.Open)))
            {
                reader.ReadLine(); //Throw out version line
                int i = 0;
                foreach (string line in GetEventStrings(reader))
                {
                    Console.WriteLine(line);
                    StoredEvent<JObject> storedevent = line.ReadStoredEvent(Source.EventSourceId, i);
                    i++;
                    Assert.That(storedevent, Is.Not.Null);
                    Assert.That(Events.Count(e => e.EventIdentifier == storedevent.EventIdentifier) == 1);
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
using System;
using Commands;
using Events;
using Ncqrs;
using Ncqrs.Commanding.ServiceModel;
using Ncqrs.Eventing.ServiceModel.Bus;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class PerformanceTests : IEventHandler<NewNoteAdded>
    {
        private ICommandService _service;
        private Guid _guid;

        [SetUp]
        public void SetUp()
        {
            BootStrapper.BootUp(this);
            _service = NcqrsEnvironment.Get<ICommandService>();
            _service.Execute(new CreateNewNote());
        }

        [Test]
        public void Test1000Commands()
        {
            var start = DateTime.Now;
            for (int i = 0; i < 10000; i++)
            {
                _service.Execute(new ChangeNoteText {NoteId= _guid, NewText = "SomeText"});
            }
            var end = DateTime.Now;
            Console.Out.WriteLine("Processed 10000 commands in = {0}", (end-start).TotalMilliseconds);
        }

        public void Handle(NewNoteAdded evnt)
        {
            _guid = evnt.NoteId;
        }
    }
}

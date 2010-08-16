using System;
using System.Threading;
using Commands;
using Events;
using Ncqrs;
using Ncqrs.Commanding.ServiceModel;
using Ncqrs.Eventing.ServiceModel.Bus;
using Ncqrs.Eventing.Storage;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class PerformanceTests : IEventHandler<NewNoteAdded>
    {
        private ICommandService _service;
        private Guid _guid;
        private Random _rand;

        [SetUp]
        public void SetUp()
        {
            BootStrapper.BootUp(this, null);
            _service = NcqrsEnvironment.Get<ICommandService>();
            _service.Execute(new CreateNewNote());
            _rand = new Random(DateTime.Now.Millisecond);
        }

        [Test]
        public void Test1000Commands()
        {
            int i, j = 0;
            for (i = 0; i < 100; i++)
            {
                for (j = 0; j < 100; j++)
                {
                    new Thread(ExecuteCommand).Start();
                    var time = -Math.Log(_rand.NextDouble()) * 10;
                    Thread.Sleep((int) time);
                }
            }
        }

        private void ExecuteCommand()
        {
            try
            {
                _service.Execute(new ChangeNoteText {NoteId = _guid, NewText = "SomeText"});
            } catch(ConcurrencyException ex)
            {
                Console.WriteLine(ex.Message);
                ExecuteCommand();
            }
        }

        public void Handle(NewNoteAdded evnt)
        {
            _guid = evnt.NoteId;
        }
    }
}

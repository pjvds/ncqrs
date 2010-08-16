using System;
using System.Threading;
using Commands;
using Events;
using Ncqrs;
using Ncqrs.Commanding.ServiceModel;
using Ncqrs.Eventing.ServiceModel.Bus;
using Ncqrs.Eventing.Storage;

namespace Tests
{
    class Program
    {
        private static ICommandService _service;
        private static Random _rand;
        private static Handler _handler;
        const int AvgRatePerSecond = 200;

        static void Main(string[] args)
        {
            _handler = new Handler();
            BootStrapper.BootUp(_handler, new TextChangedHandler());
            _service = NcqrsEnvironment.Get<ICommandService>();
            _service.Execute(new CreateNewNote());
            ExecuteCommand(0);
            _rand = new Random(DateTime.Now.Millisecond);
            int i;
            for (i = 0; i < 5000; i++)
            {
                ThreadPool.QueueUserWorkItem(cb => ExecuteCommand(0));
                var time = -(Math.Log(_rand.NextDouble()) * 1000) / AvgRatePerSecond;
                Thread.Sleep((int)time);
            }
        }

        private static void ExecuteCommand(int times)
        {
            bool executed = false;
            while (!executed)
            {
                try
                {
                    _service.Execute(new ChangeNoteText {NoteId = _handler.Guid, NewText = (times + 1).ToString()});
                    executed = true;
                }
                catch (Exception ex)
                {
                    if (!(ex is ConcurrencyException) && !(ex.InnerException is ConcurrencyException))
                        Console.WriteLine(ex.Message);
                    times++;
                    ThreadPool.QueueUserWorkItem(cb => ExecuteCommand(times));
                }
            }
        }
    }

    public class Handler : IEventHandler<NewNoteAdded>
    {
        public Guid Guid;

        public void Handle(NewNoteAdded evnt)
        {
            Guid = evnt.NoteId;
        }
    }

    public class TextChangedHandler : IEventHandler<NoteTextChanged>
    {
        private DateTime? _start;
        private int _attempts;

        public void Handle(NoteTextChanged evnt)
        {
            if (!_start.HasValue) _start = DateTime.Now;
            if ((evnt.EventSequence % 15) == 0)
            {
                _attempts += int.Parse(evnt.NewText);
                Console.WriteLine("{0} events/second, {1} attempts/command", evnt.EventSequence / (DateTime.Now - _start.Value).TotalSeconds, ((double)_attempts)/evnt.EventSequence);
            }
        }
    }
}

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
        const int AvgRatePerSecond = 100;

        static void Main(string[] args)
        {
            _handler = new Handler();
            BootStrapper.BootUp(_handler, new TextChangedHandler());
            _service = NcqrsEnvironment.Get<ICommandService>();
            _service.Execute(new CreateNewNote());
            _rand = new Random(DateTime.Now.Millisecond);
            ExecuteCommand(1);
            int i;
            for (i = 0; i < 10000; i++)
            {
                ThreadPool.QueueUserWorkItem(cb => ExecuteCommand(1));
                //The wait time is exponentially distributed, meaning that commands are Poisson distributed.
                var time = -(Math.Log(_rand.NextDouble()) * 1000) / AvgRatePerSecond;
                Thread.Sleep((int)time);
            }
        }

        private static void ExecuteCommand(int times)
        {
            try
            {
                _service.Execute(new ChangeNoteText {NoteId = _handler.Guid, NewText = times.ToString()});
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
            _attempts += int.Parse(evnt.NewText);
            if ((evnt.EventSequence % 15) == 0)
            {
                Console.WriteLine("{0} events/second, {1} attempts/command", evnt.EventSequence / (DateTime.Now - _start.Value).TotalSeconds, ((double)_attempts)/(evnt.EventSequence-1));
            }
        }
    }
}

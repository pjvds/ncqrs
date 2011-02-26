using System.Linq;
using Ncqrs.Commanding;
using NUnit.Framework;

namespace Ncqrs.Spec
{
    [Specification]
    public abstract class OneEventTestFixture<TCommand, TEvent>
        : BigBangTestFixture<TCommand>
        where TCommand : ICommand
    {

        public TEvent TheEvent { get; private set; }

        protected override void Finally()
        {
            base.Finally();
            TheEvent = PublishedEvents
                .Select(e => e.Payload)
                .OfType<TEvent>()
                .FirstOrDefault();
        }

        [Then]
        public void it_should_do_no_more()
        {
            Assert.That(PublishedEvents.Count(), Is.EqualTo(1));
        }

        [Then]
        public void the_published_event_is_not_null()
        {
            Assert.That(TheEvent, Is.Not.Null);
        }

        [Then]
        public void it_should_not_throw()
        {
            Assert.That(CaughtException, Is.Null);
        }

    }
}

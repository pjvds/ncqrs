using Ncqrs.Commanding;
using NUnit.Framework;

namespace Ncqrs.Spec
{
    [Specification]
    public abstract class ExceptionTestFixture<TCommand, TException>
        : BigBangTestFixture<TCommand>
        where TCommand : ICommand
    {

        [Then]
        public void it_should_do_nothing()
        {
            Assert.That(PublishedEvents, Is.Empty);
        }

        [Then]
        public void it_should_throw()
        {
            Assert.That(CaughtException, Is.Not.Null);
        }

        [Then]
        public void it_should_throw_the_expected_exception()
        {
            Assert.That(CaughtException, Is.InstanceOf<TException>());
        }

    }
}

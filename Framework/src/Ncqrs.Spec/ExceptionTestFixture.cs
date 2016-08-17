using Ncqrs.Commanding;
using Xunit;

namespace Ncqrs.Spec
{
    public abstract class ExceptionTestFixture<TCommand, TException>
        : BigBangTestFixture<TCommand>
        where TCommand : ICommand
    {

        [Then]
        public void it_should_do_nothing()
        {
            Assert.Empty(PublishedEvents);
        }

        [Then]
        public void it_should_throw()
        {
            Assert.NotNull(CaughtException);
        }

        [Then]
        public void it_should_throw_the_expected_exception()
        {
            Assert.IsType<TException>(CaughtException);
        }

    }
}

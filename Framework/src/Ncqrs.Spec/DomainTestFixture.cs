using System.Collections.Generic;
using Ncqrs.Commanding;
using Ncqrs.Eventing;
using Ncqrs.Spec.Fakes;

namespace Ncqrs.Spec
{

    [Specification]
    public abstract class DomainTestFixture<TCommand>
        : BaseTestFixture
        where TCommand : ICommand
    {

        private EnvironmentConfigurationWrapper _configuration;
        
        protected IEnumerable<UncommittedEvent> PublishedEvents { get; private set; }

        protected TCommand ExecutedCommand { get; private set; }

        protected abstract TCommand WhenExecuting();

        protected abstract void Execute(TCommand command);

        protected virtual void SetupDependencies()
        {
            _configuration = new EnvironmentConfigurationWrapper();
            RegisterFakesInConfiguration(_configuration);
            _configuration.Push();
        }

        protected virtual void RegisterFakesInConfiguration(EnvironmentConfigurationWrapper configuration)
        {
        }
        
        protected override void Given()
        {
            SetupDependencies();
            PublishedEvents = new UncommittedEvent[0];
            ExecutedCommand = WhenExecuting();
        }

        protected override void When()
        {
            using (var ctx = new EventContext())
            {
                Execute(ExecutedCommand);
                PublishedEvents = ctx.Events;
            }
        }

        protected override void Finally()
        {
            _configuration.Pop();
        }

    }
}

using System;
using System.Collections.Generic;
using Ncqrs.Commanding;
using Ncqrs.Commanding.ServiceModel;
using Ncqrs.Eventing;
using Ncqrs.Eventing.ServiceModel.Bus;
using Ncqrs.Eventing.Storage;
using Ncqrs.Spec.Fakes;

namespace Ncqrs.Spec
{

    [Specification]
    public abstract class BigBangTestFixture<TCommand>
        : BaseTestFixture
        where TCommand : ICommand
    {

        private EnvironmentConfigurationWrapper _configuration;
        private EventStoreWrapper _eventStore;
        private RecordingEventBus _eventBus; 

        protected Guid EventSourceId { get; private set; }
        protected TCommand ExecutedCommand { get; private set; }
        protected IEnumerable<IPublishableEvent> PublishedEvents { get; private set; }
        
        protected abstract IEnumerable<object> GivenEvents();
        protected abstract TCommand WhenExecuting();
        
        protected override void Given()
        {
            base.Given();
            GenerateEventSourceId();
            RecordGivenEvents();
            SetupRecordingEventBus();
            ReconfigureEnvironment();
        }

        protected override void When()
        {
            ExecutedCommand = WhenExecuting();
            var cmdService = NcqrsEnvironment.Get<ICommandService>();
            cmdService.Execute(ExecutedCommand);
        }

        protected override void Finally()
        {
            PublishedEvents = _eventBus.GetPublishedEvents();
            RevertConfiguration();
            base.Finally();
        }

        private void GenerateEventSourceId()
        {
            var gen = NcqrsEnvironment.Get<IUniqueIdentifierGenerator>();
            EventSourceId = gen.GenerateNewId();
        }

        private void RecordGivenEvents()
        {
            _eventStore = new EventStoreWrapper();
            _eventStore.Given(ConvertGivenEvents());
        }

        private UncommittedEventStream ConvertGivenEvents()
        {
            var history = GivenEvents();
            return Prepare.Events(history)
                .ForSourceUncomitted(EventSourceId, Guid.NewGuid());
        }

        private void SetupRecordingEventBus()
        {
            _eventBus = new RecordingEventBus();
        }

        private void ReconfigureEnvironment()
        {
            _configuration = new EnvironmentConfigurationWrapper();
            _configuration.Register<IEventStore>(_eventStore);
            _configuration.Register<IEventBus>(_eventBus);
            RegisterFakesInConfiguration(_configuration);
            _configuration.Push();
        }

        protected virtual void RegisterFakesInConfiguration(EnvironmentConfigurationWrapper configuration)
        {
        }

        private void RevertConfiguration()
        {
            _configuration.Pop();
        }

    }
}

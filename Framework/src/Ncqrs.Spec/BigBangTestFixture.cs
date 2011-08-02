using System;
using System.Collections.Generic;
using Ncqrs.Commanding;
using Ncqrs.Commanding.ServiceModel;
using Ncqrs.Eventing;
using Ncqrs.Eventing.Storage;

namespace Ncqrs.Spec
{

    public abstract class BigBangTestFixture<TCommand>
        : DomainTestFixture<TCommand>
        where TCommand : ICommand
    {

        protected Guid EventSourceId { get; private set; }

        protected ICommandService CommandService { get; private set; }

        protected virtual IEnumerable<object> GivenEvents()
        {
            return new object[0];
        }

        protected override void SetupDependencies()
        {
            base.SetupDependencies();
            GenerateEventSourceId();
            RecordGivenEvents();
            CommandService = NcqrsEnvironment.Get<ICommandService>();
        }

        protected override void Execute(TCommand command)
        {
            CommandService.Execute(command);
        }

        private void GenerateEventSourceId()
        {
            var gen = NcqrsEnvironment.Get<IUniqueIdentifierGenerator>();
            EventSourceId = gen.GenerateNewId();
        }

        private void RecordGivenEvents()
        {
            var store = NcqrsEnvironment.Get<IEventStore>();
            var givenEventStream = ConvertGivenEvents();
            store.Store(givenEventStream);
        }

        private UncommittedEventStream ConvertGivenEvents()
        {
            var history = GivenEvents();
            return Prepare.Events(history)
                .ForSourceUncomitted(EventSourceId, Guid.NewGuid());
        }

    }
}

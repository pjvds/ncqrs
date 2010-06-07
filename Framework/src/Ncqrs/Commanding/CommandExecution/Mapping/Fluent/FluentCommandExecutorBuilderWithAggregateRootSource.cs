using System;
using Ncqrs.Domain;

namespace Ncqrs.Commanding.CommandExecution.Mapping.Fluent
{
    public class FluentCommandExecutorBuilderWithAggregateRootSource<TCommand, TAggregateRoot>
        where TCommand : ICommand
        where TAggregateRoot : IAggregateRoot
    {
        public FluentAndWord<FluentDirectMethodCommandExecutorBuilderWithClosure<TCommand, TAggregateRoot>> WithId(Func<TCommand, Guid> aggregateRootIdSource)
        {
            return new FluentAndWord<FluentDirectMethodCommandExecutorBuilderWithClosure<TCommand, TAggregateRoot>>(new FluentDirectMethodCommandExecutorBuilderWithClosure<TCommand, TAggregateRoot>(aggregateRootIdSource));
        }
    }
}

using System;
using Ncqrs.Domain;

namespace Ncqrs.Commanding.CommandExecution.Mapping.Fluent
{
    public class FluentDirectMethodCommandExecutorBuilderWithClosure<TCommand, TAggregateRoot>
        where TCommand : ICommand
        where TAggregateRoot : AggregateRoot
    {
        private readonly Func<TCommand, Guid> _aggregateRootSource;

        public FluentDirectMethodCommandExecutorBuilderWithClosure(Func<TCommand, Guid> aggregateRootSource)
        {
            _aggregateRootSource = aggregateRootSource;
        }

        public Tuple<Type, ICommandExecutor> Execute(Action<TCommand, TAggregateRoot> action)
        {
            return new Tuple<Type,ICommandExecutor>(typeof(TCommand), new DirectActionOnAggregateRootCommandExecutor<TCommand, TAggregateRoot>(_aggregateRootSource, action));
        }
    }
}
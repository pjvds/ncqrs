using Ncqrs.Domain;

namespace Ncqrs.Commanding.CommandExecution.Mapping.Fluent
{
    public class FluentCommandExecutorBuilderThatMapsTo<TCommand> where TCommand : ICommand
    {
        public FluentCommandExecutorBuilderWithAggregateRootSource<TCommand, TAggregateRoot> ToContextWith<TAggregateRoot>() where TAggregateRoot : AggregateRoot
        {
            return new FluentCommandExecutorBuilderWithAggregateRootSource<TCommand,TAggregateRoot>();
        }
    }
}
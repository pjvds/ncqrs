using System;
using Ncqrs.Domain;

namespace Ncqrs.Commanding.CommandExecution
{
    public class DirectActionOnAggregateRootCommandExecutor<TCommand, TAggregateRoot> : ICommandExecutor<TCommand>
        where TCommand : ICommand
        where TAggregateRoot : AggregateRoot
    {
        private readonly Func<TCommand, Guid> _aggregateRootIdOnCommandLocator;
        private readonly Action<TCommand, TAggregateRoot> _action;

        public DirectActionOnAggregateRootCommandExecutor(Func<TCommand, Guid> aggregateRootIdOnCommandLocator, Action<TCommand, TAggregateRoot> action)
        {
            _aggregateRootIdOnCommandLocator = aggregateRootIdOnCommandLocator;
            _action = action;
        }

        public void Execute(TCommand command)
        {
            var unitOfWorkFactory = NcqrsEnvironment.Get<IUnitOfWorkFactory>();
            using (var work = unitOfWorkFactory.CreateUnitOfWork())
            {
                var id = _aggregateRootIdOnCommandLocator.Invoke(command);
                var targetAggregateRoot = work.Repository.GetById<TAggregateRoot>(id);

                _action.Invoke(command, targetAggregateRoot);

                work.Accept();
            }
        }
    }
}

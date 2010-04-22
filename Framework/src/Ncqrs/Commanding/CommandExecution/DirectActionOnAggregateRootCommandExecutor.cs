using System;
using Ncqrs.Domain;

namespace Ncqrs.Commanding.CommandExecution
{
    public class DirectActionOnAggregateRootCommandExecutor<TCommand, TAggregateRoot> : ICommandExecutor
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

        public void Execute(ICommand command)
        {
            TCommand tCommand = (TCommand)command;

            var unitOfWorkFactory = NcqrsEnvironment.Get<IUnitOfWorkFactory>();
            using (var work = unitOfWorkFactory.CreateUnitOfWork())
            {
                var id = _aggregateRootIdOnCommandLocator.Invoke(tCommand);
                var targetAggregateRoot = work.Repository.GetById<TAggregateRoot>(id);

                _action.Invoke(tCommand, targetAggregateRoot);

                work.Accept();
            }
        }
    }
}

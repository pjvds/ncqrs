using System;
using Ncqrs.Domain;

namespace Ncqrs.Commanding.CommandExecution
{
    public class DirectActionOnAggregateRootCommandExecutor<TCommand, TAggregateRoot> : ICommandExecutor<TCommand>
        where TCommand : ICommand
        where TAggregateRoot : AggregateRoot
    {
        private readonly Func<TCommand, Guid> _aggregateRootIdLocator;
        private readonly Action<TCommand, TAggregateRoot> _action;

        public DirectActionOnAggregateRootCommandExecutor(Func<TCommand, Guid> aggregateRootIdLocator, Action<TCommand, TAggregateRoot> action)
        {
            _aggregateRootIdLocator = aggregateRootIdLocator;
            _action = action;
        }

        public void Execute(TCommand command)
        {
            var unitOfWorkFactory = NcqrsEnvironment.Get<IUnitOfWorkFactory>();
            using (var work = unitOfWorkFactory.CreateUnitOfWork())
            {
                var id = _aggregateRootIdLocator.Invoke(command);
                var targetAggregateRoot = work.GetById<TAggregateRoot>(id);

                _action.Invoke(command, targetAggregateRoot);

                work.Accept();
            }
        }
    }
}

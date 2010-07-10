using System;
using Ncqrs.Domain;

namespace Ncqrs.Commanding.CommandExecution.Mapping
{
    public class DirectActionCommandExecutor<TCommand, TAggregateRoot> : ICommandExecutor<TCommand>
        where TCommand : ICommand
        where TAggregateRoot : AggregateRoot
    {
        private readonly Func<TCommand, Guid> _getId;
        private readonly Action<TAggregateRoot, TCommand> _action;

        public DirectActionCommandExecutor(Func<TCommand, Guid> getId, Action<TAggregateRoot, TCommand> action)
        {
            _getId = getId;
            _action = action;
        }

        public void Execute(TCommand command)
        {
            var unitOfWorkFactory = NcqrsEnvironment.Get<IUnitOfWorkFactory>();
            using (var work = unitOfWorkFactory.CreateUnitOfWork())
            {
                var id = _getId(command);
                var aggRoot = work.GetById<TAggregateRoot>(id);

                _action(aggRoot, command);
                work.Accept();
            }
        }
    }
}
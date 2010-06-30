using System;
using Ncqrs.Domain;

namespace Ncqrs.Commanding.CommandExecution.Mapping
{
    public class DirectMethodCommandExecutor<TCommand, TAggregateRoot> : ICommandExecutor<TCommand> where TCommand : ICommand
                                                                                                    where TAggregateRoot : AggregateRoot
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly Func<TCommand, Guid> _getAggId;
        private readonly Action<TCommand, TAggregateRoot> _action;

        public DirectMethodCommandExecutor(Func<TCommand, Guid> getAggId, Action<TCommand, TAggregateRoot> action) : this(getAggId, action, NcqrsEnvironment.Get<IUnitOfWorkFactory>())
        {
        }

        public DirectMethodCommandExecutor(Func<TCommand, Guid> getAggId, Action<TCommand, TAggregateRoot> action, IUnitOfWorkFactory uowFactory)
        {
            _getAggId = getAggId;
            _action = action;

            _uowFactory = uowFactory;
        }

        public void Execute(TCommand command)
        {
            using(var work = _uowFactory.CreateUnitOfWork())
            {
                var aggId = _getAggId(command);
                var target = work.GetById<TAggregateRoot>(aggId);

                _action(command, target);

                work.Accept();
            }
        }
    }
}

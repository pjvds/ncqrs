using System;
using Ncqrs.Domain;

namespace Ncqrs.Commanding.CommandExecution
{
    public class AggregateRootCreationCommandExecutor<TCommand> : ICommandExecutor<TCommand>
        where TCommand : ICommand
    {
        private readonly Action<TCommand> _action;

        public AggregateRootCreationCommandExecutor(Action<TCommand> action)
        {
            _action = action;
        }

        public void Execute(TCommand command)
        {
            var unitOfWorkFactory = NcqrsEnvironment.Get<IUnitOfWorkFactory>();
            using (var work = unitOfWorkFactory.CreateUnitOfWork())
            {
                _action(command);
                work.Accept();
            }
        }
    }
}

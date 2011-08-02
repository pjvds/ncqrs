using System;
using Ncqrs.Domain;

namespace Ncqrs.Commanding.CommandExecution.Mapping
{
    public class UoWMappedCommandExecutor : CommandExecutorBase<ICommand>
    {
        private readonly ICommandMapper _commandMapper;

        public UoWMappedCommandExecutor(ICommandMapper commandMapper)
        {
            _commandMapper = commandMapper;
        }

        protected override void ExecuteInContext(IUnitOfWorkContext context, ICommand command)
        {
            _commandMapper.Map(command, new UoWMappedCommandExecutorCallbacks(context, command));
        }

        private class UoWMappedCommandExecutorCallbacks : IMappedCommandExecutor
        {
            private readonly IUnitOfWorkContext _uow;
            private readonly ICommand _command;

            public UoWMappedCommandExecutorCallbacks(IUnitOfWorkContext uow, ICommand command)
            {
                _uow = uow;
                _command = command;
            }

            public void ExecuteActionOnExistingInstance(Func<ICommand, Guid> idCallback, Func<ICommand, Type> typeCallback, Action<AggregateRoot, ICommand> action)
            {
                var id = idCallback(_command);
                var type = typeCallback(_command);
                var aggRoot = _uow.GetById(type, id, _command.KnownVersion);

                action(aggRoot, _command);
                _uow.Accept();
            }

            public void ExecuteActionOnNewInstance(Action<AggregateRoot, ICommand> action)
            {
                throw new NotSupportedException();
            }

            public void ExecuteActionCreatingNewInstance(Func<ICommand, AggregateRoot> action)
            {
                action(_command);
                _uow.Accept();
            }

            public void ExecuteActionOnExistingOrCreatingNewInstance(Func<ICommand, Guid> idCallback, Func<ICommand, Type> typeCallback, Action<AggregateRoot, ICommand> existingAction, Func<ICommand, AggregateRoot> creatingAction)
            {
                var id = idCallback(_command);
                var type = typeCallback(_command);
                var aggRoot = _uow.GetById(type, id, _command.KnownVersion);

                if (aggRoot == null)
                {
                    creatingAction(_command);
                    _uow.Accept();
                }
                else
                {
                    existingAction(aggRoot, _command);
                    _uow.Accept();
                }
            }
        }
    }
}
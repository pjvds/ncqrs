using System;
using Ncqrs.Domain;

namespace Ncqrs.Commanding.CommandExecution.Mapping
{
    public interface IMappedCommandExecutor
    {
        void ExecuteActionOnExistingInstance(Func<ICommand, Guid> idCallback, Func<ICommand, Type> typeCallback, Action<AggregateRoot, ICommand> action);

        void ExecuteActionOnNewInstance(Action<AggregateRoot, ICommand> action);

        void ExecuteActionCreatingNewInstance(Func<ICommand, AggregateRoot> action);

        void ExecuteActionOnExistingOrCreatingNewInstance(Func<ICommand, Guid> idCallback, Func<ICommand, Type> typeCallback, Action<AggregateRoot, ICommand> existingAction, Func<ICommand, AggregateRoot> creatingAction);
    }

    public interface ICommandMapper
    {
        void Map(ICommand command, IMappedCommandExecutor executor);
    }
}
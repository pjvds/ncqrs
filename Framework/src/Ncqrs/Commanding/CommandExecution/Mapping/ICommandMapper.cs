using System;
using Ncqrs.Domain;

namespace Ncqrs.Commanding.CommandExecution.Mapping
{
    public interface IMappedCommandExecutor
    {
        void ExecuteActionOnExistingInstance(Func<ICommand, Guid> idCallback, Func<ICommand, Type> typeCallback, Action<AggregateRoot, ICommand> action);
        void ExecuteActionOnNewInstance(Action<AggregateRoot, ICommand> action);
        void ExecuteActionCreatingNewInstance(Func<ICommand, AggregateRoot> action);
    }

    public interface ICommandMapper
    {
        void Map(ICommand command, IMappedCommandExecutor executor);
    }
    
}
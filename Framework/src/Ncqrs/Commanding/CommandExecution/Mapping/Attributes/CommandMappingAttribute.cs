using System;

namespace Ncqrs.Commanding.CommandExecution.Mapping.Attributes
{
    public class ActionBasedCommandExecutor<TCommand> : ICommandExecutor<TCommand> where TCommand : ICommand
    {
        private readonly Action<ICommand> _action;

        public ActionBasedCommandExecutor(Action<ICommand> action)
        {
            _action = action;
        }

        public void Execute(TCommand command)
        {
            _action(command);
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public abstract class CommandMappingAttribute : Attribute
    {
        public abstract ICommandExecutor<TCommand> CreateExecutor<TCommand>() where TCommand : ICommand;
    }
}

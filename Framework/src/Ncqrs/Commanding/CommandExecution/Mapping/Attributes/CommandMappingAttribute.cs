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
        //public ICommandExecutor<ICommand> CreateExecutor(Type mappedCommandType)
        //{
        //    var method = this.GetType().GetMethod("CreateExecutor", Type.EmptyTypes);
        //    var genericMethod = method.MakeGenericMethod(mappedCommandType);

        //    // TODO: This will throw invalid cast for sure.
        //    var executor = genericMethod.Invoke(this, null);
        //    return executor;

        //    new Action
        //}

        public abstract ICommandExecutor<TCommand> CreateExecutor<TCommand>() where TCommand : ICommand;
    }
}

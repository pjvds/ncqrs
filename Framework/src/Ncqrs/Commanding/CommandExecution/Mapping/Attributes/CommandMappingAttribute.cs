using System;

namespace Ncqrs.Commanding.CommandExecution.Mapping.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public abstract class CommandMappingAttribute : Attribute
    {
        public ICommandExecutor<ICommand> CreateExecutor(Type mappedCommandType)
        {
            var method = this.GetType().GetMethod("CreateExecutor", Type.EmptyTypes);
            var genericMethod = method.MakeGenericMethod(mappedCommandType);

            // TODO: This will throw invalid cast for sure.
            return (ICommandExecutor<ICommand>)genericMethod.Invoke(this, null);
        }

        public abstract ICommandExecutor<TCommand> CreateExecutor<TCommand>() where TCommand : ICommand;
    }
}

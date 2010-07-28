using System;
using System.Diagnostics.Contracts;

namespace Ncqrs.Commanding.CommandExecution.Mapping.Attributes
{
    [ContractClass(typeof(CommandMappingAttributeContracts))]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public abstract class CommandMappingAttribute : Attribute
    {
        public abstract ICommandExecutor<TCommand> CreateExecutor<TCommand>() where TCommand : ICommand;
    }

    [ContractClassFor(typeof(CommandMappingAttribute))]
    internal class CommandMappingAttributeContracts : CommandMappingAttribute
    {
        public override ICommandExecutor<TCommand> CreateExecutor<TCommand>()
        {
            Contract.Ensures(Contract.Result<ICommandExecutor<TCommand>>() != null);
            return default(ICommandExecutor<TCommand>);
        }
    }
}

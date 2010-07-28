using System;
using System.Diagnostics.Contracts;

namespace Ncqrs.Commanding.CommandExecution.Mapping.Attributes
{
    /// <summary>
    /// Base class for attribute based command mapping. It defines
    /// a method that enables the creation of a <see cref="ICommandExecutor{TCommand}"/> 
    /// by the attribute.
    /// </summary>
    [ContractClass(typeof(CommandMappingAttributeContracts))]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public abstract class CommandMappingAttribute : Attribute
    {
        /// <summary>
        /// Creates the executor based on the mapping defined by the attribute.
        /// </summary>
        /// <typeparam name="TCommand">The type of the command. This should be the type
        /// of where the attribute was defined on.</typeparam>
        /// <returns>A new <see cref="ICommandExecutor{TCommand}"/> created based on the mapping.</returns>
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

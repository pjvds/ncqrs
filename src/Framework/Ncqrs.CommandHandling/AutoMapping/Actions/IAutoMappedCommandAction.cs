using System;

namespace Ncqrs.CommandHandling.AutoMapping.Actions
{
    /// <summary>
    /// A action that executes the needs of a mapped command.
    /// </summary>
    public interface IAutoMappedCommandAction
    {
        /// <summary>
        /// Executes this action.
        /// </summary>
        void Execute();
    }
}
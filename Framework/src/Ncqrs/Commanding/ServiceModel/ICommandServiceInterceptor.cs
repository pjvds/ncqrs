using System;

namespace Ncqrs.Commanding.ServiceModel
{
    /// <summary>
    /// Allows users to intercept commands before or after they has been executed in a command service. This allows them to add additional behavior before or after the execution of commands. 
    /// </summary>
    public interface ICommandServiceInterceptor
    {
        void OnBeforeExecution(CommandContext context);
        void OnAfterExecution(CommandContext context);
    }
}

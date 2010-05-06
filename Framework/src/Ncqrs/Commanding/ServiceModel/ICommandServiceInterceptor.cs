using System;

namespace Ncqrs.Commanding.ServiceModel
{
    public interface ICommandServiceInterceptor
    {
        void OnBeforeExecution(CommandServiceExecutionContext context);
        void OnAfterExecution(CommandServiceExecutionContext context);
    }
}

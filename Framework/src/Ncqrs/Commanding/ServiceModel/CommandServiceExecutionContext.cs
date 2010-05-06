using System;
using Ncqrs.Commanding.CommandExecution;

namespace Ncqrs.Commanding.ServiceModel
{
    public class CommandServiceExecutionContext
    {
        public Exception Exception
        {
            get; internal set;
        }

        public ICommand TheCommand
        {
            get; internal set;
        }

        public ICommandExecutor TheCommandExecutor
        {
            get; internal set;
        }

        public bool IsExecuted
        {
            get; internal set;
        }

        public CommandServiceExecutionContext(ICommand theCommand, bool isExecuted = false)
        {
            TheCommand = theCommand;
            IsExecuted = isExecuted;
        }
    }
}

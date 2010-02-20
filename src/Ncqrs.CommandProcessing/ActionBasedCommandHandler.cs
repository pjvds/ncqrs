using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Commands;

namespace Ncqrs.CommandProcessing
{
    public class ActionBasedCommandHandler : ICommandHandler
    {
        private readonly Action<ICommand> _handlingAction;

        public ActionBasedCommandHandler(Action<ICommand> handlingAction)
        {
            if (handlingAction == null) throw new ArgumentNullException("handlingAction");

            _handlingAction = handlingAction;
        }

        public void Process(ICommand command)
        {
            _handlingAction(command);
        }
    }
}

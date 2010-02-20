using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Commands;

namespace Ncqrs.CommandProcessing
{
    public interface ICommandHandlerRegister
    {
        ICommandHandler GetHandler(ICommand command);
    }
}

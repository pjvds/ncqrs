using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Commands;

namespace Ncqrs.CommandProcessing
{
    public interface ICommandService
    {
        void Process(ICommand command);

        void Process(IEnumerable<ICommand> commands);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Commands;

namespace Ncqrs.CommandProcessing
{
    public class SimpleCommandHandlerRegister : ICommandHandlerRegister
    {
        private readonly Dictionary<Type, ICommandHandler> _handlers = new Dictionary<Type, ICommandHandler>();

        public void RegisterHandler<TCommand>(ICommandHandler handler)
        {
            _handlers.Add(typeof(TCommand), handler);
        }

        public ICommandHandler GetHandler(ICommand command)
        {
            return _handlers[command.GetType()];
        }
    }
}

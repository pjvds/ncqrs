using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Commands;
using System.Diagnostics.Contracts;

namespace Ncqrs.CommandHandling
{
    public class CommandHandlerRegister : ICommandHandlerRegister
    {
        private readonly Dictionary<Type, ICommandHandler> _handlers = new Dictionary<Type, ICommandHandler>();

        public void RegisterHandler<TCommand>(ICommandHandler handler)
        {
            _handlers.Add(typeof(TCommand), handler);
        }

        public ICommandHandler GetHandler(ICommand command)
        {
            Contract.Requires<ArgumentNullException>(command != null);

            ICommandHandler handler;

            _handlers.TryGetValue(command.GetType(), out handler);

            return handler;
        }

        public bool HandlerExists(ICommand command)
        {
            Contract.Requires<ArgumentNullException>(command != null);

            return _handlers.Keys.Contains(command.GetType());
        }
    }
}

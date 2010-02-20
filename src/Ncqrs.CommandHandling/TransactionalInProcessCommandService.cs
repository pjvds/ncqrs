using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Commands;
using System.Transactions;

namespace Ncqrs.CommandHandling
{
    public class TransactionalInProcessCommandService : ICommandService
    {
        private readonly Dictionary<Type, ICommandHandler> _handlerRegister = new Dictionary<Type, ICommandHandler>();
        private readonly ICommandHandlerRegister _locator;

        public TransactionalInProcessCommandService(ICommandHandlerRegister commandHandlerLocator)
        {
            if (commandHandlerLocator == null) throw new ArgumentNullException("commandHandlerLocator");

            _locator = commandHandlerLocator;
        }

        public void Process(ICommand command)
        {
            using (var transaction = new TransactionScope())
            {
                if (command == null) throw new ArgumentNullException("command");

                ICommandHandler handler = _locator.GetHandler(command);
                handler.Handle(command);

                transaction.Complete();
            }
        }

        public void Process(IEnumerable<ICommand> commands)
        {
            using (var transaction = new TransactionScope())
            {
                foreach (var command in commands)
                {
                    Process(command);
                }

                transaction.Complete();
            }
        }
    }
}

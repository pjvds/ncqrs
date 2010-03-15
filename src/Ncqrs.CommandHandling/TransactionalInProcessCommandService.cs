using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Commands;
using System.Transactions;
using System.Diagnostics.Contracts;

namespace Ncqrs.CommandHandling
{
    public class TransactionalInProcessCommandService : ICommandService
    {
        private readonly Dictionary<Type, ICommandHandler> _handlerRegister = new Dictionary<Type, ICommandHandler>();
        private readonly ICommandHandlerRegister _locator;

        public TransactionalInProcessCommandService(ICommandHandlerRegister commandHandlerLocator)
        {
            Contract.Requires<ArgumentNullException>(commandHandlerLocator != null);

            _locator = commandHandlerLocator;
        }

        public void Execute(ICommand command)
        {
            Contract.Requires<ArgumentNullException>(command != null);

            using (var transaction = new TransactionScope())
            {
                if (command == null) throw new ArgumentNullException("command");

                ICommandHandler handler = _locator.GetHandler(command);
                handler.Execute(command);

                transaction.Complete();
            }
        }

        public void Execute(IEnumerable<ICommand> commands)
        {
            Contract.Requires(commands != null);

            using (var transaction = new TransactionScope())
            {
                foreach (var command in commands)
                {
                    Execute(command);
                }

                transaction.Complete();
            }
        }
    }
}

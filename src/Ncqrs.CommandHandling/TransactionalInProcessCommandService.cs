using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Commands;
using System.Transactions;
using System.Diagnostics.Contracts;
using System.Reflection;
using log4net;

namespace Ncqrs.CommandHandling
{
    public class TransactionalInProcessCommandService : ICommandService
    {
        private readonly Dictionary<Type, ICommandHandler> _handlers = new Dictionary<Type, ICommandHandler>();
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Execute(ICommand command)
        {
            Contract.Requires<ArgumentNullException>(command != null);
            Type commandType = command.GetType();

            Log.InfoFormat("Received {0} command and will now start processing it.", commandType.FullName);

            using (var transaction = new TransactionScope())
            {
                ICommandHandler handler = null;

                if (!_handlers.TryGetValue(commandType, out handler))
                {
                    // TODO: Add details.
                    throw new CommandHandlerNotFoundException();
                }

                Log.DebugFormat("Found commandhandler {0} to handle the {1} command. Will start executing it now.", handler.GetType().FullName, commandType.FullName);

                handler.Execute(command);

                Log.DebugFormat("Handler execution complete.");

                transaction.Complete();
            }

            Log.InfoFormat("Finished processing {0}.", commandType.FullName);
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

        public void RegisterHandler<TCommand>(ICommandHandler handler) where TCommand : ICommand
        {
            RegisterHandler(typeof(TCommand), handler);
        }

        public void RegisterHandler(Type commandType, ICommandHandler handler)
        {
            Contract.Requires<ArgumentNullException>(commandType != null);
            Contract.Requires<ArgumentNullException>(handler != null);

            _handlers.Add(commandType, handler);
        }
    }
}
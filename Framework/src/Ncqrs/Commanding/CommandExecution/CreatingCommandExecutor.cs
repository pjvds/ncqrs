using System;
using Ncqrs.Domain;
using System.Diagnostics.Contracts;

namespace Ncqrs.Commanding.CommandExecution
{
    /// <summary>
    /// A command executor that creates a <typeparamref name="TAggregateRoot"/>. The creation
    /// result will be added to the context and accepted at the end of the <see cref="Execute"/> method.
    /// </summary>
    /// <typeparam name="TCommand">The command to execute.</typeparam>
    /// <typeparam name="TAggregateRoot">The type of the object to create.</typeparam>
    public class CreatingCommandExecutor<TCommand, TAggregateRoot> : ICommandExecutor<TCommand>
        where TCommand : ICommand
        where TAggregateRoot : AggregateRoot
    {
        private readonly Func<TCommand, TAggregateRoot> _create;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreatingCommandExecutor&lt;TCommand, TAggregateRoot&gt;"/> class.
        /// </summary>
        /// <param name="create">The create func that creates the object based on the command.</param>
        public CreatingCommandExecutor(Func<TCommand, TAggregateRoot> create)
        {
            Contract.Requires<ArgumentNullException>(create != null, "The action cannot be null.");

            _create = create;
        }

        public void Execute(TCommand command)
        {
            var unitOfWorkFactory = NcqrsEnvironment.Get<IUnitOfWorkFactory>();
            using (var work = unitOfWorkFactory.CreateUnitOfWork())
            {
                _create(command);
                work.Accept();
            }
        }
    }
}
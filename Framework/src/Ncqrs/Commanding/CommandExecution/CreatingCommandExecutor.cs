using System;
using System.Diagnostics.Contracts;
using Ncqrs.Domain;

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
        private readonly IUnitOfWorkFactory _uowFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreatingCommandExecutor&lt;TCommand, TAggregateRoot&gt;"/> class.
        /// </summary>
        /// <param name="create">The create func that creates the object based on the command.</param>
        public CreatingCommandExecutor(Func<TCommand, TAggregateRoot> create) : this(create, NcqrsEnvironment.Get<IUnitOfWorkFactory>())
        {
            Contract.Requires<ArgumentNullException>(create != null, "The action cannot be null.");
        }

        public CreatingCommandExecutor(Func<TCommand, TAggregateRoot> create, IUnitOfWorkFactory uowFactory)
        {
            Contract.Requires<ArgumentNullException>(create != null, "The action cannot be null.");
            _create = create;
            _uowFactory = uowFactory;
        }

        public void Execute(TCommand command)
        {
            using (var work = _uowFactory.CreateUnitOfWork())
            {
                _create(command);
                work.Accept();
            }
        }
    }
}
using System;
using Ncqrs.Domain;
using System.Diagnostics.Contracts;

namespace Ncqrs.Commanding.CommandExecution
{
    public class DirectActionCommandExecutor<TCommand, TAggregateRoot> : ICommandExecutor<TCommand>
        where TCommand : ICommand
        where TAggregateRoot : AggregateRoot
    {
        private readonly Func<TCommand, Guid> _getId;
        private readonly Action<TAggregateRoot, TCommand> _action;
        private readonly IUnitOfWorkFactory _uowFactory;

        public DirectActionCommandExecutor(Func<TCommand, Guid> getId, Action<TAggregateRoot, TCommand> action) : this(getId, action, NcqrsEnvironment.Get<IUnitOfWorkFactory>())
        {
            Contract.Requires<ArgumentNullException>(getId != null, "The getId parameter cannot be null.");
            Contract.Requires<ArgumentNullException>(action != null, "The action parameter cannot be null.");
        }

        public DirectActionCommandExecutor(Func<TCommand, Guid> getId, Action<TAggregateRoot, TCommand> action, IUnitOfWorkFactory uowFactory)
        {
            Contract.Requires<ArgumentNullException>(getId != null, "The getId parameter cannot be null.");
            Contract.Requires<ArgumentNullException>(action != null, "The action parameter cannot be null.");
            Contract.Requires<ArgumentNullException>(uowFactory != null, "The uowFactory parameter cannot be null.");

            _getId = getId;
            _action = action;
            _uowFactory = uowFactory;
        }

        public void Execute(TCommand command)
        {
            using (var work = _uowFactory.CreateUnitOfWork())
            {
                var id = _getId(command);
                var aggRoot = work.GetById<TAggregateRoot>(id);

                _action(aggRoot, command);
                work.Accept();
            }
        }
    }
}
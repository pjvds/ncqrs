using System;
using Ncqrs.Domain;
using System.Diagnostics.Contracts;

namespace Ncqrs.Commanding.CommandExecution
{
    public class FlexibleActionCommandExecutor<TCommand, TAggregateRoot> : ICommandExecutor<TCommand>
        where TCommand : ICommand
        where TAggregateRoot : AggregateRoot
    {
        private readonly Func<TCommand, Guid> _getId;
        private readonly Func<TCommand, TAggregateRoot> _create;
        private readonly Action<TAggregateRoot, TCommand> _action;
        private readonly IUnitOfWorkFactory _uowFactory;

        public FlexibleActionCommandExecutor(Func<TCommand, Guid> getId, Func<TCommand, TAggregateRoot> create, Action<TAggregateRoot, TCommand> action)
            : this(getId, create, action, NcqrsEnvironment.Get<IUnitOfWorkFactory>())
        {
            Contract.Requires<ArgumentNullException>(getId != null, "The getId parameter cannot be null.");
            Contract.Requires<ArgumentNullException>(action != null, "The action parameter cannot be null.");
            Contract.Requires<ArgumentNullException>(create != null, "The create parameter cannot be null.");
        }

        public FlexibleActionCommandExecutor(Func<TCommand, Guid> getId, Func<TCommand, TAggregateRoot> create, Action<TAggregateRoot, TCommand> action, IUnitOfWorkFactory uowFactory)
        {
            Contract.Requires<ArgumentNullException>(getId != null, "The getId parameter cannot be null.");
            Contract.Requires<ArgumentNullException>(action != null, "The action parameter cannot be null.");
            Contract.Requires<ArgumentNullException>(create != null, "The create parameter cannot be null.");
            Contract.Requires<ArgumentNullException>(uowFactory != null, "The uowFactory parameter cannot be null.");

            _getId = getId;
            _action = action;
            _create = create;
            _uowFactory = uowFactory;
        }

        public void Execute(TCommand command)
        {
            using (var work = _uowFactory.CreateUnitOfWork())
            {
                var id = _getId(command);
                try
                {
                    var aggRoot = work.GetById<TAggregateRoot>(id);

                    _action(aggRoot, command);
                    work.Accept();
                }
                catch (Ncqrs.Domain.Storage.AggregateRootNotFoundException)
                {
                    _create(command);
                    work.Accept();
                }
            }
        }
    }
}
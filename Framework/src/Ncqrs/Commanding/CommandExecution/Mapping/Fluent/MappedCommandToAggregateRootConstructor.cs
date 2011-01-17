using System;
using System.Diagnostics.Contracts;
using Ncqrs.Domain;

namespace Ncqrs.Commanding.CommandExecution.Mapping.Fluent
{
    /// <summary>
    /// Represents the creation of an aggregateroot of type <typeparamref name="TAggRoot"/> from a command of type <typeparamref name="TCommand"/>.
    /// </summary>
    /// <typeparam name="TCommand">The command that triggers the creation of the aggregateroot of type <typeparamref name="TAggRoot"/>.</typeparam>
    /// <typeparam name="TAggRoot">The aggregateroot of type <typeparamref name="TAggRoot"/> on which we executed the command.</typeparam>
    public class MappedCommandToAggregateRootConstructor<TCommand, TAggRoot> : ICommandExecutor<TCommand> where TCommand : ICommand where TAggRoot : AggregateRoot
    {
        private readonly Func<TCommand, TAggRoot> _aggregaterootcreatorfunc;
        private Action<TCommand, TAggRoot> _storeaggregaterootinfunc;

        /// <summary>
        /// Constructor, initializes a new instance of <see cref="MappedCommandToAggregateRootConstructor&lt;TCommand, TAggRoot&gt;"/>.
        /// </summary>
        /// <param name="creatorfunc">The method that is responsible for the creation of the aggregateroot.</param>
        /// <remarks>Marked as internal because the construction is only allowed in the framework.</remarks>
        internal MappedCommandToAggregateRootConstructor(Func<TCommand, TAggRoot> creatorfunc)
        {
            Contract.Requires<ArgumentNullException>(creatorfunc != null, "creatorfunc can not be null.");
            _aggregaterootcreatorfunc = creatorfunc;
        }

        /// <summary>
        /// Defines an action where in the created aggregateroot can be stored.
        /// </summary>
        /// <param name="storeaggregaterootinfunc">The action that defines the storage of the created aggregateroot.</param>
        /// <returns>The <see cref="ICommandExecutor&lt;TCommand&gt;"/> which is able to execute this command.</returns>
        public ICommandExecutor<TCommand> StoreIn(Action<TCommand, TAggRoot> storeaggregaterootinfunc)
        {
            Contract.Requires<ArgumentNullException>(storeaggregaterootinfunc != null, "storeaggregaterootinfunc can not be null.");
            _storeaggregaterootinfunc = storeaggregaterootinfunc;

            return this;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        void ICommandExecutor<TCommand>.Execute(TCommand command)
        {
            var factory = NcqrsEnvironment.Get<IUnitOfWorkFactory>();
            using (var work = factory.CreateUnitOfWork(command.CommandIdentifier))
            {
                var aggregateroot = _aggregaterootcreatorfunc(command);

                if (_storeaggregaterootinfunc != null)
                    _storeaggregaterootinfunc(command, aggregateroot);

                work.Accept();
            }
        }
    }
}
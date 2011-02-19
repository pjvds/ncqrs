using System;
using System.Diagnostics.Contracts;
using Ncqrs.Domain;

namespace Ncqrs.Commanding.CommandExecution.Mapping.Fluent
{
    /// <summary>
    /// Represents the method that is to be called on the aggregateroot of type <typeparamref name="TAggRoot"/>.
    /// </summary>
    /// <typeparam name="TCommand">The command of type <typeparamref name="TCommand"/> that is executing the method.</typeparam>
    /// <typeparam name="TAggRoot">The aggregateroot of type <typeparamref name="TAggRoot"/> on which we execute the method.</typeparam>
    public class MappedCommandToAggregateRootMethod<TCommand, TAggRoot> : ICommandExecutor<TCommand> where TCommand : ICommand where TAggRoot : AggregateRoot
    {
        private readonly Func<Guid, long?, TAggRoot> _aggregaterootfetchfunc;
        private readonly Func<TCommand, Guid> _getidfromcommandfunc;
        private Action<TCommand, TAggRoot> _mappedmethodforcommandfunc;

        /// <summary>
        /// Constructor, initializes a new instance of <see cref="MappedCommandToAggregateRootMethod&lt;TCommand, TAggRoot&gt;"/>.
        /// </summary>
        /// <param name="getidfromcommandfunc">The method responsible for retrieving the id of the aggregateroot from the command.</param>
        /// <remarks>Marked as internal because the construction is only allowed in the framework.</remarks>
        internal MappedCommandToAggregateRootMethod(Func<TCommand, Guid> getidfromcommandfunc)
        {
            _getidfromcommandfunc = getidfromcommandfunc;
            _aggregaterootfetchfunc = delegate(Guid guid, long? lastKnownVersion)
                                          {
                                              IUnitOfWorkContext uow = UnitOfWorkContext.Current;
                                              return (TAggRoot) uow.GetById(typeof (TAggRoot), guid, lastKnownVersion);
                                          };
        }

        /// <summary>
        /// Constructor, initializes a new instance of <see cref="MappedCommandToAggregateRootMethod&lt;TCommand, TAggRoot&gt;"/>.
        /// </summary>
        /// <param name="getidfromcommandfunc">The method responsible for retrieving the id of the aggregateroot from the command.</param>
        /// <param name="aggregaterootfetchfunc">The method responsible for retrieving the aggregateroot of type <typeparamref name="TAggRoot"/> from a <see cref="Guid"/>.</param>
        /// <remarks>Marked as internal because the construction is only allowed in the framework.</remarks>
        internal MappedCommandToAggregateRootMethod(Func<Guid, long?, TAggRoot> aggregaterootfetchfunc, Func<TCommand, Guid> getidfromcommandfunc) 
            : this(getidfromcommandfunc)
        {
            _aggregaterootfetchfunc = aggregaterootfetchfunc;
        }

        /// <summary>
        /// Takes a method that is responsible for calling the method on the aggregateroot of type <typeparamref name="TAggRoot"/> 
        /// with any parameters from the command of type <typeparamref name="TCommand"/>.
        /// </summary>
        /// <param name="commandmappedfunc">The method that is responsible for the method mapping.</param>
        /// <returns>The <see cref="ICommandExecutor&lt;TCommand&gt;"/> which is able to execute this command.</returns>
        public ICommandExecutor<TCommand> ToCallOn(Action<TCommand, TAggRoot> commandmappedfunc)
        {
            Contract.Requires<ArgumentNullException>(commandmappedfunc != null, "commandmappedfunc can not be null.");
            _mappedmethodforcommandfunc = commandmappedfunc;

            return this;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <returns>The aggregateroot of type <typeparamref name="TAggRoot"/> on which we executed the command.</returns>
        void ICommandExecutor<TCommand>.Execute(TCommand command)
        {
            var factory = NcqrsEnvironment.Get<IUnitOfWorkFactory>();
            using (var work = factory.CreateUnitOfWork(command.CommandIdentifier))
            {
                var aggregateroot = _aggregaterootfetchfunc(_getidfromcommandfunc(command), command.KnownVersion);
                _mappedmethodforcommandfunc(command, aggregateroot);

                work.Accept();
            }
        }

        [ContractInvariantMethod]
        private void ContractInvariants()
        {
            Contract.Invariant(_getidfromcommandfunc != null, "getidfromcommandfunc can not be null.");
            Contract.Invariant(_aggregaterootfetchfunc != null, "aggregaterootfetchfunc can not be null.");
        }
    }
}
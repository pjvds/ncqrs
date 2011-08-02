using System;
using System.Diagnostics.Contracts;
using Ncqrs.Domain;

namespace Ncqrs.Commanding.CommandExecution.Mapping.Fluent
{
    /// <summary>
    /// Represents the base mapping of a command.
    /// </summary>
    /// <typeparam name="TCommand">The mapped command to be executed for for the aggregateroot.</typeparam>
    public abstract class MappedCommandToAggregateRoot<TCommand> where TCommand : ICommand
    {
        /// <summary>
        /// Executes the given command of type <typeparamref name="TCommand"/> for the mapped aggregateroot.
        /// </summary>
        /// <param name="command">The <see cref="TCommand"/> to execute.</param>
        internal abstract void Execute(TCommand command);
    }

    /// <summary>
    /// Represents a mapping between the command type <typeparamref name="TCommand"/> and the aggregateroot type <typeparamref name="TAggRoot"/>.
    /// </summary>
    /// <typeparam name="TCommand">The mapped command to be executed for for the aggregateroot.</typeparam>
    /// <typeparam name="TAggRoot">The aggregateroot in which we will execute the given command.</typeparam>
    public class MappedCommandToAggregateRoot<TCommand, TAggRoot> : MappedCommandToAggregateRoot<TCommand>
        where TCommand : ICommand
        where TAggRoot : AggregateRoot
    {
        private ICommandExecutor<TCommand> _mappedaggregaterootmethod;

        /// <summary>
        /// Constructor, initializes a new instance of <see cref="MappedCommandToAggregateRoot&lt;TCommand, TAggRoot&gt;"/>.
        /// </summary>
        /// <remarks>Marked as internal because the construction is only allowed in the framework.</remarks>
        internal MappedCommandToAggregateRoot()
        { }

        /// <summary>
        /// Executes the given command of type <typeparamref name="TCommand"/> for the mapped aggregateroot.
        /// </summary>
        /// <param name="command">The <see cref="TCommand"/> to execute.</param>
        internal override void Execute(TCommand command)
        {
            Contract.Requires<ArgumentNullException>(command != null, "command can not be null.");

            _mappedaggregaterootmethod.Execute(command);
        }

        /// <summary>
        /// Creates a <see cref="MappedCommandToAggregateRootMethod&lt;TCommand, TAggRoot&gt;"/> which represents the method that is to be
        /// called on the aggregateroot of type <typeparamref name="TAggRoot"/>
        /// </summary>
        /// <param name="getidfromcommandfunc">The method responsible for retrieving the id of the aggregateroot from the command.</param>
        /// <returns>A <see cref="MappedCommandToAggregateRootMethod&lt;TCommand, TAggRoot&gt;"/>.</returns>
        public MappedCommandToAggregateRootMethod<TCommand, TAggRoot> WithId(Func<TCommand, Guid> getidfromcommandfunc)
        {
            Contract.Requires<ArgumentNullException>(getidfromcommandfunc != null, "getidfromcommandfunc can not be null.");
            var mappedmethod = new MappedCommandToAggregateRootMethod<TCommand, TAggRoot>(getidfromcommandfunc);

            _mappedaggregaterootmethod = mappedmethod;
            return mappedmethod;
        }

        /// <summary>
        /// Creates a <see cref="MappedCommandToAggregateRootMethod&lt;TCommand, TAggRoot&gt;"/> which represents the method that is to be called
        /// on the aggregateroot of type <typeparamref name="TAggRoot"/>.
        /// </summary>
        /// <param name="getidfromcommandfunc">The method responsible for retrieving the id of the aggregateroot from the command.</param>
        /// <param name="getaggregaterootfunc">The method responsible for retrieving the aggregateroot of type <typeparamref name="TAggRoot"/> from a <see cref="Guid"/>.</param>
        /// <returns>A <see cref="MappedCommandToAggregateRootMethod&lt;TCommand, TAggRoot&gt;"/>.</returns>
        public MappedCommandToAggregateRootMethod<TCommand, TAggRoot> WithId(Func<TCommand, Guid> getidfromcommandfunc, Func<Guid, long?, TAggRoot> getaggregaterootfunc)
        {
            Contract.Requires<ArgumentNullException>(getidfromcommandfunc != null, "getidfromcommandfunc can not be null.");
            Contract.Requires<ArgumentNullException>(getaggregaterootfunc != null, "getaggregaterootfunc can not be null.");

            var mappedmethod = new MappedCommandToAggregateRootMethod<TCommand, TAggRoot>(getaggregaterootfunc, getidfromcommandfunc);
            _mappedaggregaterootmethod = mappedmethod;

            return mappedmethod;
        }

        /// <summary>
        /// Creates a <see cref="MappedCommandToAggregateRootMethodOrConstructor&lt;TCommand, TAggRoot&gt;"/> which represents the method that is to be
        /// called on the aggregateroot of type <typeparamref name="TAggRoot"/> to use an existing aggregate root or create a new root if needed
        /// </summary>
        /// <param name="getidfromcommandfunc">The method responsible for retrieving the id of the aggregateroot from the command.</param>
        /// <param name="aggregaterootcreatorfunc">The method responsible for creating the aggregateroot of type <typeparamref name="TAggRoot"/>.</param>
        /// <returns>A <see cref="MappedCommandToAggregateRootMethodOrConstructor&lt;TCommand, TAggRoot&gt;"/>.</returns>
        public MappedCommandToAggregateRootMethodOrConstructor<TCommand, TAggRoot> UseExistingOrCreateNew(Func<TCommand, Guid> getidfromcommandfunc, Func<TCommand, TAggRoot> aggregaterootcreatorfunc)
        {
            Contract.Requires<ArgumentNullException>(getidfromcommandfunc != null, "getidfromcommandfunc can not be null.");
            Contract.Requires<ArgumentNullException>(aggregaterootcreatorfunc != null, "aggregaterootcreatorfunc can not be null.");
            var mappedmethod = new MappedCommandToAggregateRootMethodOrConstructor<TCommand, TAggRoot>(getidfromcommandfunc, aggregaterootcreatorfunc);

            _mappedaggregaterootmethod = mappedmethod;
            return mappedmethod;
        }

        /// <summary>
        /// Creates a <see cref="MappedCommandToAggregateRootMethodOrConstructor&lt;TCommand, TAggRoot&gt;"/> which represents the method that is to be
        /// called on the aggregateroot of type <typeparamref name="TAggRoot"/> to use an existing aggregate root or create a new root if needed
        /// </summary>
        /// <param name="getidfromcommandfunc">The method responsible for retrieving the id of the aggregateroot from the command.</param>
        /// <param name="aggregaterootcreatorfunc">The method responsible for creating the aggregateroot of type <typeparamref name="TAggRoot"/>.</param>
        /// <returns>A <see cref="MappedCommandToAggregateRootMethodOrConstructor&lt;TCommand, TAggRoot&gt;"/>.</returns>
        public MappedCommandToAggregateRootMethodOrConstructor<TCommand, TAggRoot> UseExistingOrCreateNew(Func<TCommand, Guid> getidfromcommandfunc, Func<Guid, long?, TAggRoot> getaggregaterootfunc, Func<TCommand, TAggRoot> aggregaterootcreatorfunc)
        {
            Contract.Requires<ArgumentNullException>(getidfromcommandfunc != null, "getidfromcommandfunc can not be null.");
            Contract.Requires<ArgumentNullException>(aggregaterootcreatorfunc != null, "aggregaterootcreatorfunc can not be null.");
            Contract.Requires<ArgumentNullException>(getaggregaterootfunc != null, "getaggregaterootfunc can not be null.");

            var mappedmethod = new MappedCommandToAggregateRootMethodOrConstructor<TCommand, TAggRoot>(getidfromcommandfunc, getaggregaterootfunc, aggregaterootcreatorfunc);

            _mappedaggregaterootmethod = mappedmethod;
            return mappedmethod;
        }

        /// <summary>
        /// Creates a <see cref="MappedCommandToAggregateRootConstructor&lt;Command, TAggRoot&gt;"/> which represents the creation of
        /// an aggregateroot of type <typeparamref name="TAggRoot"/>.
        /// </summary>
        /// <param name="aggregaterootcreatorfunc">The method responsible for creating the aggregateroot of type <typeparamref name="TAggRoot"/>.</param>
        /// <returns>A <see cref="MappedCommandToAggregateRootConstructor&lt;Command, TAggRoot&gt;"/>.</returns>
        public MappedCommandToAggregateRootConstructor<TCommand, TAggRoot> CreateNew(Func<TCommand, TAggRoot> aggregaterootcreatorfunc)
        {
            Contract.Requires<ArgumentNullException>(aggregaterootcreatorfunc != null, "aggregaterootcreatorfunc can not be null.");

            var mappedcreator = new MappedCommandToAggregateRootConstructor<TCommand, TAggRoot>(aggregaterootcreatorfunc);
            _mappedaggregaterootmethod = mappedcreator;

            return mappedcreator;
        }
    }
}
using Ncqrs.Domain;

namespace Ncqrs.Commanding.CommandExecution.Mapping.Fluent
{
    /// <summary>
    /// Represents a command of type <see cref="TCommand"/> that is to be mapped.
    /// </summary>
    /// <typeparam name="TCommand">The command on which we want to map.</typeparam>
    public class MappedCommand<TCommand> : ICommandExecutor<TCommand> where TCommand : ICommand
    {
        private MappedCommandToAggregateRoot<TCommand> _executablecommand;

        /// <summary>
        /// Constructor, initializes a new instance of <see cref="MappedCommand&lt;TCommand&gt;"/>.
        /// </summary>
        /// <remarks>Marked as internal because the construction is only allowed in the framework.</remarks>
        internal MappedCommand()
        {}

        /// <summary>
        /// Creates a <see cref="MappedCommandToAggregateRoot&lt;TCommand, TAggRoot&gt;"/> which represents the mapping between
        /// the command of type <typeparamref name="TCommand"/> and the aggregateroot of type <typeparamref name="TAggRoot"/>.
        /// </summary>
        /// <typeparam name="TAggRoot">The type of the aggregateroot in which we want to map.</typeparam>
        /// <returns>A <see cref="MappedCommandToAggregateRoot&lt;TCommand, TAggRoot&gt;"/>.</returns>
        public MappedCommandToAggregateRoot<TCommand, TAggRoot> ToAggregateRoot<TAggRoot>() where TAggRoot : AggregateRoot
        {
            var mappedcommand = new MappedCommandToAggregateRoot<TCommand, TAggRoot>();
            _executablecommand = mappedcommand;

            return mappedcommand;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <exception cref="ArgumentNullException">Occurs when <i>command</i> is null.</exception>
        void ICommandExecutor<TCommand>.Execute(TCommand command)
        {
            _executablecommand.Execute(command);
        }
    }
}
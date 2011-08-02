using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;

namespace Ncqrs.Commanding.CommandExecution.Mapping.Fluent
{
    /// <summary>
    /// Static Map class that defines easy to use command mapping methods.
    /// </summary>
    public static class Map
    {
        /// <summary>
        /// Creates a new <see cref="MappedCommand&lt;TCommand&gt;"/> which can be used for fluent mapping.
        /// </summary>
        /// <typeparam name="TCommand">The type of command.</typeparam>
        /// <returns>A <see cref="MappedCommand&lt;TCommand&gt;"/>.</returns>
        public static MappedCommand<TCommand> Command<TCommand>() where TCommand : ICommand
        {
            return new MappedCommand<TCommand>();
        }

        ///// <summary>
        ///// Returns a <see cref="ICommandExecutor&lt;TCommand&gt;"/> for attribute based mappings.
        ///// </summary>
        ///// <typeparam name="TCommand">The type of command.</typeparam>
        ///// <returns>An <see cref="ICommandExecutor&lt;TCommand&gt;"/>.</returns>
        //public static ICommandExecutor<TCommand> Command<TCommand>(AttributeMappedCommandExecutor<TCommand> mappedexecutor) where TCommand : ICommand
        //{
        //    return mappedexecutor;
        //}
    }
}
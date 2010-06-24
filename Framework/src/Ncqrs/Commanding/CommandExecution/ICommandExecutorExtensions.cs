using Ncqrs.Commanding.ServiceModel;

namespace Ncqrs.Commanding.CommandExecution
{
    /// <summary>
    /// Defines an easy to use extension method for the <see cref="ICommandExecutor{TCommand}"/> interface.
    /// </summary>
    public static class ICommandExecutorExtensions
    {
        /// <summary>
        /// Registers the executor to the <see cref="CommandService"/> service.
        /// </summary>
        /// <typeparam name="TCommand">The type of command to register to the service.</typeparam>
        /// <param name="executor">The executor to register with the command.</param>
        /// <param name="service">The service on which we want to register the executor.</param>
        public static void RegisterWith<TCommand>(this ICommandExecutor<TCommand> executor, CommandService service) where TCommand : ICommand
        {
            service.RegisterExecutor(executor);
        }
    }
}
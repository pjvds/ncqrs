using System;

namespace Ncqrs.Commanding.CommandExecution.Mapping.Fluent
{
    public class Map
    {
        public static FluentCommandExecutorBuilderThatMapsTo<TCommand> Command<TCommand>() where TCommand : ICommand
        {
            return new FluentCommandExecutorBuilderThatMapsTo<TCommand>();
        }
    }
}

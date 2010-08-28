using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;

namespace Ncqrs.Spec
{
    [Specification]
    public abstract class AutoMappedCommandTestFixture<TCommand> : CommandTestFixture<TCommand> 
        where TCommand : ICommand 
    {
        private readonly AttributeBasedMappingFactory _factory = new AttributeBasedMappingFactory();

        protected override ICommandExecutor<TCommand> BuildCommandExecutor()
        {
            return _factory.CreateExecutorForCommand<TCommand>();
        }
    }
}

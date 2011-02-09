using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution;
using Ncqrs.Commanding.CommandExecution.Mapping;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;

namespace Ncqrs.Spec
{
    [Specification]
    public abstract class AutoMappedCommandTestFixture<TCommand> : CommandTestFixture<TCommand> 
        where TCommand : ICommand 
    {
        private readonly AttributeBasedCommandMapper _mapper = new AttributeBasedCommandMapper();

        protected override ICommandExecutor<ICommand> BuildCommandExecutor()
        {
            return new UoWMappedCommandExecutor(_mapper);
        }
    }
}

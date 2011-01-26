using System;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution;
using Ncqrs.Commanding.CommandExecution.Mapping;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;
using Ncqrs.Commanding.ServiceModel;

namespace Ncqrs.NServiceBus
{
    /// <summary>
    /// Ncqrs command service integrated with NServiceBus. 
    /// </summary>
    public class NsbCommandService : CommandService
    {
        private readonly ICommandExecutor<ICommand> _executor;
        private readonly AttributeBasedCommandMapper _mapper;

        public NsbCommandService()
        {
            _mapper = new AttributeBasedCommandMapper();
            _executor = new UoWMappedCommandExecutor(_mapper);
        }

        public new void RegisterExecutor<TCommand>(ICommandExecutor<TCommand> executor) where TCommand : ICommand
        {
            base.RegisterExecutor(executor);
        }
        protected override Action<ICommand> GetCommandExecutorForCommand(Type commandType)
        {
            var registeredExecutor = base.GetCommandExecutorForCommand(commandType);
            if (registeredExecutor == null)
            {
                if (_mapper.CanMapCommand(commandType))
                {
                    registeredExecutor = x => _executor.Execute(x);
                }
            }
            return registeredExecutor;
        }
    }
}
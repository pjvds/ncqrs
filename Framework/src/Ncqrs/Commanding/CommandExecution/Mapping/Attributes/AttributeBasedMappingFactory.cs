using System;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution;

namespace Ncqrs.Commanding.CommandExecution.Mapping.Attributes
{
    public class AttributeBasedMappingFactory
    {
        public ICommandExecutor<TCommand> CreateMappingForCommand<TCommand>() where TCommand : ICommand
        {
            var commandType = typeof (TCommand);

            if(IsMappedToCtor(commandType))
            {
                CreateForMappedToCtor(commandType);
            }
            else if(IsMappedToMethod(commandType))
            {
                CreateForMappedToMethod(commandType);
            }
            else
            {
                var msg = string.Format("Could not create executor for command {0} based on "+
                    "attribute mapping. It does not contain a MapsToAggregateRootConstructorAttribute "+
                    "or MapsToAggregateRootMethodAttribute.", commandType.FullName);

                throw new CommandMappingException(msg);
            }
        }

        private void CreateForMappedToMethod(Type commandType)
        {
            throw new NotImplementedException();
        }

        private void CreateForMappedToCtor(Type commandType)
        {
            throw new NotImplementedException();
        }

        private static bool IsMappedToCtor(Type commandType)
        {
            return commandType.IsDefined(typeof(MapsToAggregateRootConstructorAttribute), false);
        }

        private static bool IsMappedToMethod(Type commandType)
        {
            return commandType.IsDefined(typeof(MapsToAggregateRootMethodAttribute), false);            
        }
    }
}

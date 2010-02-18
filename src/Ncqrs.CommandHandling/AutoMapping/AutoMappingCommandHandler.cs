using System;
using Ncqrs.Commands;
using Ncqrs.Domain.Storage;

namespace Ncqrs.CommandHandling.AutoMapping
{
    public class AutoMappingCommandHandler<T> : CommandHandler<T> where T : ICommand
    {
        public AutoMappingCommandHandler(IDomainRepository domainRepository) : base(domainRepository)
        {
            
        }

        public override void Handle(T command)
        {
            var factory = new ActionFactory(Repository);
            var action = factory.CreateActionForCommand(command);
            action.Execute();
        }
    }
}

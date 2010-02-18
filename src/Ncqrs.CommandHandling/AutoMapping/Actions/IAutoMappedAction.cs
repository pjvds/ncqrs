using System;

namespace Ncqrs.CommandHandling.AutoMapping.Actions
{
    public interface IAutoMappedAction
    {
        void Execute();
    }
}
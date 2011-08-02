using System;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution.Mapping;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;
using Ncqrs.Domain;

namespace Ncqrs.Tests.Commanding.CommandExecution.Mapping.Attributes
{
    public class TestAttributeMappedCommandExecutor<T> : IMappedCommandExecutor
        where T : AggregateRoot
    {
        public T Instance { get; set; }

        public ICommand Command { get; set; }

        public Action VerificationAction { get; set; }

        public TestAttributeMappedCommandExecutor()
        {
        }

        public TestAttributeMappedCommandExecutor(ICommand command)
        {
            Command = command;
        }

        public TestAttributeMappedCommandExecutor(ICommand command, T instance)
        {
            Command = command;
            Instance = instance;
        }

        public void Execute()
        {
            new AttributeBasedCommandMapper().Map(Command, this);
        }

        public void ExecuteActionOnExistingInstance(Func<ICommand, Guid> idCallback, Func<ICommand, Type> typeCallback, Action<AggregateRoot, ICommand> action)
        {
            if (VerificationAction != null)
            {
                VerificationAction();
            }
            action(Instance, Command);
        }

        public void ExecuteActionOnNewInstance(Action<AggregateRoot, ICommand> action)
        {
            throw new NotImplementedException();
        }

        public void ExecuteActionCreatingNewInstance(Func<ICommand, AggregateRoot> action)
        {
            if (VerificationAction != null)
            {
                VerificationAction();
            }
            Instance = (T)action(Command);
        }

        public void ExecuteActionOnExistingOrCreatingNewInstance(Func<ICommand, Guid> idCallback, Func<ICommand, Type> typeCallback, Action<AggregateRoot, ICommand> existingAction, Func<ICommand, AggregateRoot> creatingAction)
        {
            if (VerificationAction != null)
            {
                VerificationAction();
            }
            if (Instance == null)
            {
                Instance = (T)creatingAction(Command);
            }
            else
            {
                existingAction(Instance, Command);
            }
        }
    }
}
using System;
using System.Linq;
using System.Reflection;
using Ncqrs.Commands;
using Ncqrs.Domain;
using Ncqrs.Domain.Storage;
using System.Collections.Generic;

namespace Ncqrs.CommandHandling.AutoMapping.Actions
{
    public class ObjectCreationAction : IAutoMappedAction
    {
        private readonly ICommand _command;
        private readonly IDomainRepository _repository;
        private readonly ObjectCreationCommandInfo _commandInfo;

        public ObjectCreationAction(IDomainRepository repository, ICommand command)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            if (command == null) throw new ArgumentNullException("command");

            _repository = repository;
            _command = command;
            _commandInfo = ObjectCreationCommandInfo.CreateFromDirectMethodCommand(command);
        }

        public void Execute()
        {
            using (var work = new UnitOfWork(_repository))
            {
                var config = new AutoMapperConfiguration();
                var targetCtor = GetConstructorBasedOnCommand();

                var parameterValues = config.GetParameterValues(_command, targetCtor.GetParameters());
                targetCtor.Invoke(parameterValues);

                work.Accept();
            }
        }

        private ConstructorInfo GetConstructorBasedOnCommand()
        {
            var config = new AutoMapperConfiguration();
            var aggregateType = _commandInfo.AggregateType;
            var propertiesToMap = config.GetCommandProperties(_command);
            var ctorQuery = from ctor in aggregateType.GetConstructors()
                            where ctor.GetParameters().Length == propertiesToMap.Count()
                            where ParametersDoMatchPropertiesToMap(ctor.GetParameters(), propertiesToMap)
                            select ctor;

            if (ctorQuery.Count() == 0)
            {
                var message = String.Format("No constructor found with {0} parameters on aggregate root {1}.",
                                            propertiesToMap.Count(), aggregateType.FullName);
                throw new CommandMappingException(message);
            }
            if (ctorQuery.Count() > 1)
            {
                var message = String.Format("Multiple constructors found with {0} parameters on aggregate root {1}.",
                                            propertiesToMap.Count(), aggregateType.FullName);
                throw new CommandMappingException(message);
            }

            return ctorQuery.First();
        }

        // TODO: Remove this duplicate method.
        private Boolean ParametersDoMatchPropertiesToMap(ParameterInfo[] parameterInfo, IEnumerable<PropertyInfo> propertiesToMap)
        {
            var enumerator = propertiesToMap.GetEnumerator();

            for (int i = 0; i < parameterInfo.Length; i++)
            {
                if (!enumerator.MoveNext())
                {
                    return false;
                }

                if (!parameterInfo[i].ParameterType.IsAssignableFrom(enumerator.Current.PropertyType))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
using System;
using System.Linq;
using System.Reflection;
using Ncqrs.Commands;
using Ncqrs.Domain.Storage;
using Ncqrs.Domain;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Ncqrs.CommandHandling.AutoMapping.Actions
{
    public class DirectMethodAction : IAutoMappedAction
    {
        private readonly ICommand _command;
        private readonly DirectMethodCommandInfo _info;
        private readonly IDomainRepository _repository;

        public DirectMethodAction(IDomainRepository repository, ICommand command)
        {
            Contract.Requires<ArgumentNullException>(repository != null, "The parameter repository should not be null.");
            Contract.Requires<ArgumentNullException>(command != null, "The parameter command should not be null.");

            Contract.Ensures(_repository == repository, "The field _repository should be initialized by the parameter value of repository.");
            Contract.Ensures(_command == command, "The field _command should be initialized by the parameter value of command.");

            _repository = repository;
            _command = command;
            _info = DirectMethodCommandInfo.CreateFromDirectMethodCommand(command);
        }

        public void Execute()
        {
            using (var work = new UnitOfWork(_repository))
            {
                var config = new AutoMapperConfiguration();
                var targetMethod = GetTargetMethodBasedOnCommandTypeName();

                var parameterValues = config.GetParameterValues(_command, targetMethod.GetParameters());
                var targetAggregateRoot = _repository.GetById(_info.AggregateType, _info.AggregateRootIdValue);

                targetMethod.Invoke(targetAggregateRoot, parameterValues);

                work.Accept();
            }
        }

        private MethodInfo GetTargetMethodBasedOnCommandTypeName()
        {
            var config = new AutoMapperConfiguration();
            var aggregateType = _info.AggregateType;
            var propertiesToMap = config.GetCommandProperties(_command);
            var ctorQuery = from method in aggregateType.GetMethods()
                            where method.Name == _info.MethodName
                            where method.GetParameters().Length == propertiesToMap.Count()
                            where ParametersDoMatchPropertiesToMap(method.GetParameters(), propertiesToMap)
                            select method;

            if (ctorQuery.Count() == 0)
            {
                var message = String.Format("No method '{0}' found with {1} parameters on aggregate root {2}.",
                                            _info.MethodName, propertiesToMap.Count(), aggregateType.FullName);
                throw new CommandMappingException(message);
            }
            if (ctorQuery.Count() > 1)
            {
                var message = String.Format("Multiple methods '{0}' found with {1} parameters on aggregate root {2}.",
                                            _info.MethodName, propertiesToMap.Count(), aggregateType.FullName);
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
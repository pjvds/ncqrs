using System;
using System.Linq;
using System.Reflection;
using Ncqrs.Commands;
using Ncqrs.Domain.Storage;
using Ncqrs.Domain;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Ncqrs.Config;

namespace Ncqrs.CommandExecution.AutoMapping.Actions
{
    /// <summary>
    /// An auto mapped action that executes a method on an aggregate root based on the mapping specified on the command.
    /// </summary>
    public class DirectMethodAction : ICommandExecutor
    {
        /// <summary>
        /// Executes this method on the aggregate root based on the mapping of the command given a construction time.
        /// </summary>
        public void Execute(ICommand command)
        {
            var info = DirectMethodCommandInfo.CreateFromDirectMethodCommand(command);

            var factory = NcqrsEnvironment.Get<IUnitOfWorkFactory>();

            using (var work = factory.CreateUnitOfWork())
            {
                var targetMethod = GetTargetMethodBasedOnCommandTypeName(info, command);

                var parameterValues = CommandAutoMappingConfiguration.GetParameterValues(command, targetMethod.GetParameters());
                var targetAggregateRoot = work.Repository.GetById(info.AggregateType, info.AggregateRootIdValue);

                targetMethod.Invoke(targetAggregateRoot, parameterValues);

                work.Accept();
            }
        }

        private MethodInfo GetTargetMethodBasedOnCommandTypeName(DirectMethodCommandInfo info, ICommand command)
        {
            var aggregateType = info.AggregateType;
            var propertiesToMap = CommandAutoMappingConfiguration.GetCommandProperties(command);
            var ctorQuery = from method in aggregateType.GetMethods()
                            where method.Name == info.MethodName
                            where method.GetParameters().Length == propertiesToMap.Count()
                            where ParametersDoMatchPropertiesToMap(method.GetParameters(), propertiesToMap)
                            select method;

            if (ctorQuery.Count() == 0)
            {
                var message = String.Format("No method '{0}' found with {1} parameters on aggregate root {2}.",
                                            info.MethodName, propertiesToMap.Count(), aggregateType.FullName);
                throw new CommandMappingException(message);
            }
            if (ctorQuery.Count() > 1)
            {
                var message = String.Format("Multiple methods '{0}' found with {1} parameters on aggregate root {2}.",
                                            info.MethodName, propertiesToMap.Count(), aggregateType.FullName);
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
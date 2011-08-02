using System;
using System.Linq;
using System.Reflection;
using Ncqrs.Domain;

namespace Ncqrs.Commanding.CommandExecution.Mapping.Attributes
{
    public class MapsToAggregateRootMethodOrConstructorAttributeHandler : IMappingAttributeHandler<MapsToAggregateRootMethodOrConstructorAttribute>
    {
        public void Map(MapsToAggregateRootMethodOrConstructorAttribute attribute, ICommand command, IMappedCommandExecutor executor)
        {
            var commandType = command.GetType();
            ValidateCommandType(commandType);

            var match = GetMatchingMethod(attribute, commandType, attribute.MethodName);

            Action<AggregateRoot, ICommand> existingAction =
                (agg, cmd) =>
                {
                    var parameter = match.Item2.Select(p => p.GetValue(cmd, null));
                    match.Item1.Invoke(agg, parameter.ToArray());
                };

            var creatingMatch = GetMatchingConstructor(attribute, commandType);
            Func<ICommand, AggregateRoot> create = (c) =>
            {
                var parameter = match.Item2.Select(p => p.GetValue(c, null));
                return (AggregateRoot)creatingMatch.Item1.Invoke(parameter.ToArray());
            };

            Action executorAction = () => executor.ExecuteActionOnExistingOrCreatingNewInstance(GetAggregateRootId, GetAggregateRootType, existingAction, create);

            if (commandType.IsDefined(typeof(TransactionalAttribute), false))
            {
                var transactionService = NcqrsEnvironment.Get<ITransactionService>();
                transactionService.ExecuteInTransaction(executorAction);
            }
            else
            {
                executorAction();
            }
        }

        private static Tuple<MethodInfo, PropertyInfo[]> GetMatchingMethod(MapsToAggregateRootMethodOrConstructorAttribute attribute, Type commandType, string methodName)
        {
            var strategy = new AttributePropertyMappingStrategy();
            var sources = strategy.GetMappedProperties(commandType);

            return PropertiesToMethodMapper.GetMethod(sources, attribute.Type, methodName);
        }

        private static Tuple<ConstructorInfo, PropertyInfo[]> GetMatchingConstructor(MapsToAggregateRootMethodOrConstructorAttribute attribute, Type commandType)
        {
            var strategy = new AttributePropertyMappingStrategy();
            var sources = strategy.GetMappedProperties(commandType);

            return PropertiesToMethodMapper.GetConstructor(sources, attribute.Type);
        }

        private static Guid GetAggregateRootId(ICommand cmd)
        {
            const BindingFlags allInstance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            var commandType = cmd.GetType();
            var idProp = commandType.GetProperties(allInstance).Single(p => p.IsDefined(typeof(AggregateRootIdAttribute), false));
            return (Guid)idProp.GetValue(cmd, null);
        }

        private static Type GetAggregateRootType(ICommand cmd)
        {
            var commandType = cmd.GetType();
            var attribute =
                commandType.GetCustomAttributes(typeof(MapsToAggregateRootMethodOrConstructorAttribute), false).Cast<MapsToAggregateRootMethodOrConstructorAttribute>
                    ().Single();
            return attribute.Type;
        }

        private static void ValidateCommandType(Type mappedCommandType)
        {
            bool containsThisAttribute = mappedCommandType.IsDefined(typeof(MapsToAggregateRootMethodOrConstructorAttribute), false);

            if (!containsThisAttribute) throw new ArgumentException("The given command type does not contain " +
                                                                    "MapsToAggregateRootMethodOrConstructorAttribute.",
                                                                    "mappedCommandType");
        }
    }
}
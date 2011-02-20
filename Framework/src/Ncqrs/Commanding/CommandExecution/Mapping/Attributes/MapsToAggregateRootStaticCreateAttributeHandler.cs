using System;
using System.Linq;
using System.Reflection;
using Ncqrs.Domain;

namespace Ncqrs.Commanding.CommandExecution.Mapping.Attributes
{
    public class MapsToAggregateRootStaticCreateAttributeHandler : IMappingAttributeHandler<MapsToAggregateRootStaticCreateAttribute>
    {
        public void Map(MapsToAggregateRootStaticCreateAttribute attribute, ICommand command, IMappedCommandExecutor executor)
        {
            var commandType = command.GetType();
            ValidateCommandType(commandType);

            var match = GetMatchingMethod(attribute, commandType, attribute.MethodName);

            Action<AggregateRoot, ICommand> action =
                (agg, cmd) =>
                    {
                        var parameter = match.Item2.Select(p => p.GetValue(cmd, null));
                        match.Item1.Invoke(agg, parameter.ToArray());
                    };

            Action executorAction = () => executor.ExecuteActionOnExistingInstance(GetAggregateRootId, GetAggregateRootType, action);

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

        private static Tuple<MethodInfo, PropertyInfo[]> GetMatchingMethod(MapsToAggregateRootStaticCreateAttribute attribute, Type commandType, string methodName)
        {
            var strategy = new AttributePropertyMappingStrategy();
            var sources = strategy.GetMappedProperties(commandType);
            var bindingFlags = BindingFlags.Public | BindingFlags.Static;

            return PropertiesToMethodMapper.GetMethod(sources, attribute.Type, bindingFlags, methodName);
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
                commandType.GetCustomAttributes(typeof(MapsToAggregateRootStaticCreateAttribute), false).Cast<MapsToAggregateRootStaticCreateAttribute>
                    ().Single();
            return attribute.Type;
        }

        private static void ValidateCommandType(Type mappedCommandType)
        {
            bool containsThisAttribute = mappedCommandType.IsDefined(typeof(MapsToAggregateRootStaticCreateAttribute), false);

            if (!containsThisAttribute) throw new ArgumentException("The given command type does not contain " +
                                                                    "MapsToAggregateRootConstructorAttribute.",
                                                                    "mappedCommandType");
        }
    }
}
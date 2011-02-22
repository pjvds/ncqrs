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

            Func<ICommand, AggregateRoot> create = (c) =>
            {
                var parameter = match.Item2.Select(p => p.GetValue(c, null));
                return
                    (AggregateRoot)match.Item1.Invoke(null, parameter.ToArray());
            };

            Action executorAction = () => executor.ExecuteActionCreatingNewInstance(create);

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

        private static void ValidateCommandType(Type mappedCommandType)
        {
            bool containsThisAttribute = mappedCommandType.IsDefined(typeof(MapsToAggregateRootStaticCreateAttribute), false);

            if (!containsThisAttribute) throw new ArgumentException("The given command type does not contain " +
                                                                    "MapsToAggregateRootConstructorAttribute.",
                                                                    "mappedCommandType");
        }
    }
}
using System;
using System.Linq;
using System.Reflection;
using Ncqrs.Domain;

namespace Ncqrs.Commanding.CommandExecution.Mapping.Attributes
{
    public class MapsToAggregateRootConstructorAttributeHandler : IMappingAttributeHandler<MapsToAggregateRootConstructorAttribute>
    {
        public void Map(MapsToAggregateRootConstructorAttribute attribute, ICommand command, IMappedCommandExecutor executor)
        {
            var commandType = command.GetType();
            ValidateCommandType(commandType);

            var match = GetMatchingConstructor(attribute, commandType);
            Func<ICommand, AggregateRoot> create = (c) =>
                                                       {
                                                           var parameter = match.Item2.Select(p => p.GetValue(c, null));
                                                           return (AggregateRoot)match.Item1.Invoke(parameter.ToArray());
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

        private static Tuple<ConstructorInfo, PropertyInfo[]> GetMatchingConstructor(MapsToAggregateRootConstructorAttribute attribute, Type commandType)
        {
            var strategy = new AttributePropertyMappingStrategy();
            var sources = strategy.GetMappedProperties(commandType);

            return PropertiesToMethodMapper.GetConstructor(sources, attribute.Type);
        }

        private static void ValidateCommandType(Type mappedCommandType)
        {
            var expectedAttribute = typeof (MapsToAggregateRootConstructorAttribute);
            bool containsThisAttribute = mappedCommandType.IsDefined(expectedAttribute, false);

            if (!containsThisAttribute) throw new ArgumentException("The given command type does not contain " +
                                                                    expectedAttribute.FullName + ".",
                                                                    "mappedCommandType");
        }
    }
}
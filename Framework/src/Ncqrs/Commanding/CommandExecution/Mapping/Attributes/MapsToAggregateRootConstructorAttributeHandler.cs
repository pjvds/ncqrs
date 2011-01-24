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

            executor.ExecuteActionCreatingNewInstance(create);
        }

        private static Tuple<ConstructorInfo, PropertyInfo[]> GetMatchingConstructor(MapsToAggregateRootConstructorAttribute attribute, Type commandType)
        {
            var strategy = new AttributePropertyMappingStrategy();
            var sources = strategy.GetMappedProperties(commandType);

            return PropertiesToMethodMapper.GetConstructor(sources, attribute.Type);
        }

        private static void ValidateCommandType(Type mappedCommandType)
        {
            bool containsThisAttribute = mappedCommandType.IsDefined(typeof(MapsToAggregateRootConstructorAttribute), false);

            if (!containsThisAttribute) throw new ArgumentException("The given command type does not contain " +
                                                                    "MapsToAggregateRootConstructorAttribute.",
                                                                    "mappedCommandType");
        }
    }
}
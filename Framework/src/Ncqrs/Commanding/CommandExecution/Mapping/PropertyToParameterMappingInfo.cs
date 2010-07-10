using System.Reflection;

namespace Ncqrs.Commanding.CommandExecution.Mapping
{
    public class PropertyToParameterMappingInfo
    {
        public int? Ordinal
        {
            get; private set;
        }

        public string TargetName
        {
            get; private set;
        }

        public PropertyInfo Property
        {
            get; private set;
        }

        public PropertyToParameterMappingInfo(int? ordinal, string name, PropertyInfo property)
        {
            Ordinal = ordinal;
            TargetName = name;
            Property = property;
        }
    }
}
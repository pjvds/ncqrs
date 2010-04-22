using System;

namespace Ncqrs.Commanding.CommandExecution.Mapping.Attributes
{
    /// <summary>
    /// Marks a property as the property that contains the aggregate root id.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class AggregateRootIdAttribute : ExcludeInMappingAttribute
    {
    }
}
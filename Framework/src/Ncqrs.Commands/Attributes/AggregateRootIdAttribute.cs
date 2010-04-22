using System;

namespace Ncqrs.Commands.Attributes
{
    /// <summary>
    /// Marks a property as the propery that contains the aggregate root id.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class AggregateRootIdAttribute : ExcludeInMappingAttribute
    {
    }
}
using System;

namespace Ncqrs.Commands.AutoMapping
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class AggregateRootIdAttribute : ExcludeInMappingAttribute
    {
    }
}
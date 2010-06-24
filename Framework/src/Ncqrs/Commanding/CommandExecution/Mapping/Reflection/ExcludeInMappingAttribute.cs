using System;

namespace Ncqrs.Commanding.CommandExecution.Mapping.Reflection
{
    /// <summary>
    /// Defines that a property should be excluded in the auto mapping process.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExcludeInMappingAttribute : Attribute
    {
    }
}
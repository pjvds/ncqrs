using System;

namespace Ncqrs.Commands.Attributes
{
    /// <summary>
    /// Defines that a property should be excluded in the auto mapping process.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ExcludeInMappingAttribute : Attribute
    {
    }
}
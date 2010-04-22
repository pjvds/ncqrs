using System;

namespace Ncqrs.Eventing.ServiceModel.Bus.Mapping
{
    /// <summary>
    /// Specifies that the method is not an event handler. Use this attribute to exclude the
    /// method from the <see cref="ConventionBasedInternalEventHandlerMappingStrategy"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class NoEventHandlerAttribute : Attribute
    {
    }
}

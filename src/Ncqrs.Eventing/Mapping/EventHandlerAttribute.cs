using System;

namespace Ncqrs.Eventing.Mapping
{
    [AttributeUsage(AttributeTargets.Method)]
    public class EventHandlerAttribute : Attribute
    {
    }
}
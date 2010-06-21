using System;
using Ncqrs.Eventing.Storage;

namespace AwesomeAppRefactored.Events
{
    public class AppV2EventsTypeResolver : IPropertyBagTypeResolver
    {
        public Type Resolve(string className, string nameSpace, string assemblyName)
        {
            var oldNameSpace = "AwesomeApp.Events";

            if(nameSpace == oldNameSpace)
            {
                // We have an event of the old namespace here.
                switch (className)
                {
                    // Fix typo of event class name.
                    case "NameChangedEventttt":
                        return typeof (NameChangedEvent);
                    
                    // Only fix namespace move.
                    case "PersonCreatedEvent":
                        return typeof (PersonCreatedEvent);
                }
            }

            return null;
        }
    }
}

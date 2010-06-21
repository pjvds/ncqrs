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
                    case "NameChangedEvent":
                        return typeof (NameChangedEvent);
                    case "PersonCreatedEvent":
                        return typeof (PersonCreatedEvent);
                }
            }

            return null;
        }
    }
}

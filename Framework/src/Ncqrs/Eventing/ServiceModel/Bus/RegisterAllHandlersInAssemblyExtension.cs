using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

namespace Ncqrs.Eventing.ServiceModel.Bus
{
    public static class RegisterAllHandlersInAssemblyExtension
    {
        private static ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void RegisterAllHandlersInAssembly(this InProcessEventBus target, Assembly asm)
        {
            target.RegisterAllHandlersInAssembly(asm, CreateInstance);
        }

        public static void RegisterAllHandlersInAssembly(this InProcessEventBus target, Assembly asm, Func<Type, object> handlerFactory)
        {
            foreach(var type in asm.GetTypes().Where(ImplementsAtLeastOneIEventHandlerInterface))
            {
                var handler = handlerFactory(type);

                foreach(var handlerInterfaceType in type.GetInterfaces().Where(IsIEventHandlerInterface))
                {
                    var eventDataType = handlerInterfaceType.GetGenericArguments().First();
                    RegisterHandler(handler, eventDataType, target);
                }
            }
        }

        private static object CreateInstance(Type type)
        {
            Contract.Requires<ArgumentNullException>(type != null);
            return Activator.CreateInstance(type);
        }

        private static void RegisterHandler(object handler, Type eventDataType, InProcessEventBus target)
        {
            var registerHandlerMethod = target.GetType().GetMethods().Single
            (
                m => m.Name == "RegisterHandler" && m.IsGenericMethod && m.GetParameters().Count() == 1
            );

            var targetMethod = registerHandlerMethod.MakeGenericMethod(new[] { eventDataType });
            targetMethod.Invoke(target, new object[] { handler });

            _log.InfoFormat("Registered {0} as event handler for event {1}.", handler.GetType().FullName, eventDataType.FullName);
        }

        private static bool ImplementsAtLeastOneIEventHandlerInterface(Type type)
        {
            return type.IsClass && !type.IsAbstract &&
                   type.GetInterfaces().Any(IsIEventHandlerInterface);
        }

        private static bool IsIEventHandlerInterface(Type type)
        {
            return type.IsInterface &&
                   type.IsGenericType &&
                   type.GetGenericTypeDefinition() == typeof (IEventHandler<>);
        }
    }
}

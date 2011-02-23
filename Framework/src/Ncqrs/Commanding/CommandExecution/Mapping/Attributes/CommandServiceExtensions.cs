using System.Linq;
using System.Reflection;
using Ncqrs.Commanding.ServiceModel;
using System;

namespace Ncqrs.Commanding.CommandExecution.Mapping.Attributes
{
    public static class RegisterExecutorForAllMappedCommandsInAssemblyExtension
    {
        private static ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void RegisterExecutorsInAssembly(this CommandService target, Assembly asm)
        {
            RegisterExecutorsForCommandsMappedByAttributes(target, asm);
            RegisterExecutorsForCustomCommandExecutors(target, asm);
        }
              
        private static void RegisterExecutorsForCommandsMappedByAttributes(CommandService target, Assembly asm)
        {
            var factory = new AttributeBasedMappingFactory();

            foreach (var mappedCommand in asm.GetTypes().Where(t => factory.IsCommandMapped(t)))
            {
                var executor = factory.CreateExecutorForCommand(mappedCommand);
                target.RegisterExecutor(mappedCommand, executor);
            }
        }

        private static void RegisterExecutorsForCustomCommandExecutors(CommandService target, Assembly asm)
        {
            var executorTypes = asm.GetTypes()
                .Where(x => IsConcrete(x))
                .Select(x => new { ExecutorType = x, ExecutorInterfaces = GetExecutorInterfaces(x) })
                .Where(x => x.ExecutorInterfaces.Count() > 0);

            foreach (var executorType in executorTypes)
            {
                dynamic executor = Activator.CreateInstance(executorType.ExecutorType); //we're getting the command type from the executor itself, so dynamic is safe (+ saves hideous reflection + performance penalty)

                foreach (var executorInterface in executorType.ExecutorInterfaces)
                {
                    var commandType = executorInterface.GetGenericArguments().First(); //the command type is always the first generic argument

                    target.RegisterExecutor(commandType, executor);
                }
            }
        }

        static Type _executorType = typeof(ICommandExecutor<>);

        private static System.Collections.Generic.IEnumerable<Type> GetExecutorInterfaces(Type executorType)
        {
            return executorType.GetInterfaces().Where(y => y.GetGenericTypeDefinition() == _executorType);
        }

        private static bool IsConcrete(Type x)
        {
            return x.IsAbstract == false && x.IsInterface == false;
        }
        
    }
}

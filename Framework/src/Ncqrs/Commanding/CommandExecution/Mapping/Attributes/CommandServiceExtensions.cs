using System;
using System.Linq;
using System.Reflection;
using Ncqrs.Commanding.ServiceModel;

namespace Ncqrs.Commanding.CommandExecution.Mapping.Attributes
{
    public static class RegisterExecutorForAllMappedCommandsInAssemblyExtension
    {
        private static ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void RegisterExecutorsInAssembly(this CommandService target, Assembly asm)
        {
            var mapper = new AttributeBasedCommandMapper();

            foreach (var mappedCommand in asm.GetTypes().Where(mapper.CanMapCommand))
            {
                var executor = new UoWMappedCommandExecutor(mapper);
                target.RegisterExecutor(mappedCommand, executor);
            }
        }
    }

    public static class RegisterAllExplicitCommandExecutorsInAssemblyExtension
    {
        public static void RegisterAllExplicitCommandExecutorsInAssembly(this CommandService target, Assembly asm)
        {
            var targetType = typeof(ICommandExecutor<>);
            var types = asm.GetExportedTypes();
            var commandExecutorTypes = types
                .Where(x =>
                    x.IsInterface == false &&
                    x.IsAbstract == false &&
                    x.GetInterface(targetType.FullName) != null);

            foreach (var executorType in commandExecutorTypes)
            {
                dynamic executor = Activator.CreateInstance(executorType, true);
                var commandType = executorType.GetInterface(targetType.FullName).GetGenericArguments().First();

                target.RegisterExecutor(commandType, executor);
            }
        }
    }
}

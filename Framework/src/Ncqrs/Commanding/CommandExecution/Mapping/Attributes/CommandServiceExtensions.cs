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
            var factory = new AttributeBasedMappingFactory();

            foreach(var mappedCommand in asm.GetTypes().Where(t=>factory.IsCommandMapped(t)))
            {
                var executor = factory.CreateExecutorForCommand(mappedCommand);
                target.RegisterExecutor(mappedCommand, executor);
            }
        }
    }
}

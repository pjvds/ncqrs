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

            foreach(var mappedCommand in asm.GetTypes().Where(mapper.CanMapCommand))
            {
                var executor = new UoWMappedCommandExecutor(mapper);
                target.RegisterExecutor(mappedCommand, executor);
            }
        }
    }
}

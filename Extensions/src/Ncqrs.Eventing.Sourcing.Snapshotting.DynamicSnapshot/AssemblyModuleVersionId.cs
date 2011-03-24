using System;
using System.Reflection;
using System.IO;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{
    public static class AssemblyModuleVersionId
    {
        public static void Change(string assemblyFileName, string moduleName, Guid newModuleVersionId)
        {
            var assemblyData = File.ReadAllBytes(assemblyFileName);
            var currentMVID = Assembly.Load(assemblyData).GetModule(moduleName).ModuleVersionId.ToByteArray();
            var newMVID = newModuleVersionId.ToByteArray();

            if (assemblyData.Replace(currentMVID, newMVID))
            {
                File.WriteAllBytes(assemblyFileName, assemblyData);
            }
        }



    }
}

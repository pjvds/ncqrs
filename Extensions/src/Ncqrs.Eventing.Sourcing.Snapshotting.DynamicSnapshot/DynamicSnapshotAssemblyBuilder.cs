using System;
using System.Reflection.Emit;
using System.Reflection;
using System.Collections.Generic;
using System.IO;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{
    internal sealed class DynamicSnapshotAssemblyBuilder
    {

        #region Constants

        public const string DefaultModuleName = "DynamicSnapshot.dll";

        #endregion

        #region Fields

        private readonly AssemblyBuilder _assemblyBuilder;

        private readonly string _assemblyFileName;

        private readonly ModuleBuilder _moduleBuilder;

        private readonly object _padlock = new object();

        private readonly Dictionary<Type, Type> _typeRegistry = new Dictionary<Type, Type>();

        #endregion

        #region Constructors

        public DynamicSnapshotAssemblyBuilder(AssemblyName assemblyName)
            : this(assemblyName, DefaultModuleName)
        { }

        public DynamicSnapshotAssemblyBuilder(AssemblyName assemblyName, string moduleName)
        {
            _assemblyFileName = moduleName;
            _assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule(moduleName, _assemblyFileName);
        }

        public DynamicSnapshotAssemblyBuilder()
            : this(new AssemblyName(DefaultModuleName), DefaultModuleName)
        { }

        #endregion

        #region Public Methods

        public Type GetSnapshotType(Type sourceType)
        {
            Type result;
            _typeRegistry.TryGetValue(sourceType, out result);
            return result;
        }

        public Type RegisterSnapshotType(Type sourceType)
        {
            if (sourceType == null)
                throw new ArgumentNullException("sourceType");

            var snapshotType = GetSnapshotType(sourceType);
            if (snapshotType != null)
                return snapshotType;

            snapshotType = DynamicSnapshotTypeBuilder.CreateType(sourceType, _moduleBuilder);

            if (snapshotType != null)
            {
                lock (_padlock)
                {
                    _typeRegistry.Add(sourceType, snapshotType);
                }
            }

            return snapshotType;
        }

        public Assembly SaveAssembly()
        {
            var file = _assemblyFileName;

            //if (!file.EndsWith(".dll")) 
                //file = string.Format("{0}.dll", file);

            if (string.Compare(Path.GetExtension(_assemblyFileName), ".dll", true) != 0)
                file = Path.ChangeExtension(_assemblyFileName, ".dll");

            if (File.Exists(file)) 
                File.Delete(file);

            _assemblyBuilder.Save(file);

            return _assemblyBuilder;
        }

        #endregion

    }

}

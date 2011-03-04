using System;
using System.Reflection.Emit;
using System.Reflection;
using System.Collections.Generic;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{
    internal sealed class SnapshotAssemblyBuilder
    {
        #region Constants

        private const string DefaultModuleName = "SnapshotModule";

        #endregion

        #region Fields

        private readonly AssemblyBuilder _assemblyBuilder;

        private readonly ModuleBuilder _moduleBuilder;

        private readonly object _padlock = new object();

        private readonly Dictionary<Type, Type> _typeRegistry = new Dictionary<Type, Type>();

        #endregion

        #region Constructors

        public SnapshotAssemblyBuilder(AssemblyName assemblyName, string moduleName)
        {
            _assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule(moduleName, true); // TODO: true?
        }

        public SnapshotAssemblyBuilder(AssemblyName assemblyName)
            : this(assemblyName, DefaultModuleName)
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

            snapshotType = SnapshotTypeBuilder.CreateType(sourceType, _moduleBuilder);

            if (snapshotType != null)
            {
                lock (_padlock)
                {
                    _typeRegistry.Add(sourceType, snapshotType);
                }
            }

            return snapshotType;
        }

        public void SaveAssembly(string path)
        {
            _assemblyBuilder.Save(path);
        }

        #endregion

    }

}

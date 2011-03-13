using System;
using System.Reflection.Emit;
using System.Reflection;
using System.Collections.Generic;
using System.IO;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{
    /// <summary>
    /// Class for building snapshot assembly.
    /// </summary>
    internal sealed class DynamicSnapshotAssemblyBuilder
    {
        private const string AssemblyFileExtension = ".dll";

        public const string DefaultModuleName = "DynamicSnapshot.dll";

        private readonly AssemblyBuilder _assemblyBuilder;

        private readonly string _assemblyFileName;

        private readonly ModuleBuilder _moduleBuilder;

        private readonly object _padlock = new object();

        private readonly DynamicSnapshotTypeBuilder _typeBuilder;

        private readonly Dictionary<Type, Type> _typeRegistry = new Dictionary<Type, Type>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicSnapshotAssemblyBuilder"/> class.
        /// </summary>
        /// <param name="typeBuilder">The type builder.</param>
        public DynamicSnapshotAssemblyBuilder(DynamicSnapshotTypeBuilder typeBuilder)
        {
            _typeBuilder = typeBuilder;
            var assemblyName = new AssemblyName(DefaultModuleName);
            _assemblyFileName = DefaultModuleName;
            _assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule(DefaultModuleName, _assemblyFileName);
        }

        /// <summary>
        /// Registers a snapshot type for given aggregate.
        /// </summary>
        /// <param name="aggregateType">Type of the aggregate.</param>
        /// <returns></returns>
        public Type RegisterSnapshotType(Type aggregateType)
        {
            if (aggregateType == null)
                throw new ArgumentNullException("sourceType");

            var snapshotType = GetSnapshotType(aggregateType);
            if (snapshotType != null)
                return snapshotType;

            snapshotType = _typeBuilder.CreateType(aggregateType, _moduleBuilder);

            if (snapshotType != null)
            {
                lock (_padlock)
                {
                    _typeRegistry.Add(aggregateType, snapshotType);
                }
            }

            return snapshotType;
        }

        /// <summary>
        /// Saves the assembly to the file system.
        /// </summary>
        /// <returns></returns>
        public Assembly SaveAssembly()
        {
            var file = _assemblyFileName;

            if (string.Compare(Path.GetExtension(_assemblyFileName), AssemblyFileExtension, true) != 0)
                file = Path.ChangeExtension(_assemblyFileName, AssemblyFileExtension);

            if (File.Exists(file))
                File.Delete(file);

            _assemblyBuilder.Save(file);

            return _assemblyBuilder;
        }

        private Type GetSnapshotType(Type sourceType)
        {
            Type result;
            _typeRegistry.TryGetValue(sourceType, out result);
            return result;
        }

    }

}

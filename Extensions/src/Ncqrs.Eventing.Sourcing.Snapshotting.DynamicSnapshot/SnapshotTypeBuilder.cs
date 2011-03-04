using System;
using System.Reflection.Emit;
using System.Reflection;
using System.Collections.Generic;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{
    public static class SnapshotTypeBuilder
    {
        #region Readonly

        private static readonly Type _snapshotBaseType = typeof(DynamicSnapshotBase);

        #endregion

        #region Public Methods

        public static Type CreateType(Type sourceType, ModuleBuilder moduleBuilder)
        {
            if (sourceType == null) throw new ArgumentNullException("sourceType");
            if (moduleBuilder == null) throw new ArgumentNullException("moduleBuilder");

            Guard(sourceType);

            var typeBuilder = GetTypeBuilder(sourceType, moduleBuilder);
            CreateConstructor(sourceType, typeBuilder);
            CreateFields(sourceType, typeBuilder);

            return typeBuilder.CreateType();
        }

        #endregion

        #region Private Methods

        private static void CreateConstructor(Type sourceType, TypeBuilder typeBuilder)
        {
            var snapshotBaseCtor = _snapshotBaseType.GetConstructors(BindingFlags.Public | BindingFlags.Instance).First();
            var snapshotBaseCtorSignature = snapshotBaseCtor.GetParameters().Select(p => p.ParameterType);
            var snapshotCtorSignature = new List<Type>(snapshotBaseCtorSignature);
            snapshotCtorSignature.Add(sourceType);

            var ctorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, snapshotCtorSignature.ToArray());
            var il = ctorBuilder.GetILGenerator();

            var initializer = typeof(DynamicSnapshot).GetMethod("InitializeFrom");

            il.Emit(OpCodes.Ldarg_0); // push this

            int paramIndex = 1;
            foreach (var param in snapshotBaseCtorSignature)
            {
                il.Emit(OpCodes.Ldarg, paramIndex++);
            }

            il.Emit(OpCodes.Call, snapshotBaseCtor); // call Snapshot constructor

            il.Emit(OpCodes.Ldarg_0); // push this
            il.Emit(OpCodes.Ldarg, paramIndex); // push source
            il.Emit(OpCodes.Call, initializer); // call InitializeFrom(this, source)
            il.Emit(OpCodes.Ret); // return
        }

        private static void CreateFields(Type sourceType, TypeBuilder typeBuilder)
        {
            var fieldMap = SnapshotableField.GetMap(sourceType);
            foreach (var pair in fieldMap)
            {
                typeBuilder.DefineField(pair.Key, pair.Value.FieldType, FieldAttributes.Public);
            }
        }

        private static string GenerateSnapshotClassName(Type sourceType)
        {
            return string.Format("{0}_Snapshot", sourceType.Name);
        }

        private static TypeBuilder GetTypeBuilder(Type sourceType, ModuleBuilder moduleBuilder)
        {
            return moduleBuilder.DefineType(
                    GenerateSnapshotClassName(sourceType),
                    TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Serializable | TypeAttributes.Sealed,
                    _snapshotBaseType);
        }

        private static void Guard(Type sourceType)
        {
            bool isSnapshotable = sourceType.Inherits<AggregateRoot>() && sourceType.HasAttribute<DynamicSnapshotAttribute>();

            if (!isSnapshotable)
                throw new DynamicSnapshotsNotSupportedException() { Source = sourceType };
        }

        #endregion

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Ncqrs.Domain;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{
    internal static class DynamicSnapshotTypeBuilder
    {
        #region Readonly

        private static readonly Type SnapshotBaseType = typeof(DynamicSnapshotBase);

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
            var ctor = SnapshotBaseType.GetConstructors(BindingFlags.Public | BindingFlags.Instance).First();
            var ctorSignature = ctor.GetParameters().Select(p => p.ParameterType);

            var ctorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, ctorSignature.ToArray());
            var il = ctorBuilder.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0); // push this

            int paramIndex = 1;
            foreach (var param in ctorSignature) // push all constructor parameters onto the stack
            {
                il.Emit(OpCodes.Ldarg, paramIndex++);
            }

            il.Emit(OpCodes.Call, ctor); // call DynamicSnapshotBase constructor
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
                    SnapshotBaseType);
        }

        private static void Guard(Type sourceType)
        {
            bool isSnapshotable = typeof(AggregateRoot).IsAssignableFrom(sourceType) && sourceType.HasAttribute<DynamicSnapshotAttribute>();

            if (!isSnapshotable)
                throw new DynamicSnapshotNotSupportedException() { AggregateType = sourceType };
        }

        #endregion

    }
}
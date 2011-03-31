using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Ncqrs.Domain;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{
    internal class DynamicSnapshotTypeBuilder
    {
        private readonly Type SnapshotBaseType = typeof(DynamicSnapshotBase);

        /// <summary>
        /// Creates a snapshot type from aggregate.
        /// </summary>
        /// <param name="aggregateType">Type of the aggregate.</param>
        /// <param name="moduleBuilder">The module builder.</param>
        /// <returns></returns>
        public Type CreateType(Type aggregateType, ModuleBuilder moduleBuilder)
        {
            if (aggregateType == null) throw new ArgumentNullException("sourceType");
            if (moduleBuilder == null) throw new ArgumentNullException("moduleBuilder");

            Guard(aggregateType);

            var typeBuilder = GetTypeBuilder(aggregateType, moduleBuilder);
            CreateConstructor(typeBuilder);
            CreateFields(aggregateType, typeBuilder);

            return typeBuilder.CreateType();
        }

        private void CreateConstructor(TypeBuilder typeBuilder)
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

        private void CreateFields(Type sourceType, TypeBuilder typeBuilder)
        {
            var fieldMap = SnapshotableField.GetMap(sourceType);
            foreach (var pair in fieldMap)
            {
                typeBuilder.DefineField(pair.Key, pair.Value.FieldType, FieldAttributes.Public);
            }
        }

        private TypeBuilder GetTypeBuilder(Type sourceType, ModuleBuilder moduleBuilder)
        {
            return moduleBuilder.DefineType(
                    SnapshotNameGenerator.Generate(sourceType),
                    TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Serializable | TypeAttributes.Sealed,
                    SnapshotBaseType);
        }

        private void Guard(Type sourceType)
        {
            bool isSnapshotable = typeof(AggregateRoot).IsAssignableFrom(sourceType) && sourceType.HasAttribute<DynamicSnapshotAttribute>();

            if (!isSnapshotable)
                throw new DynamicSnapshotNotSupportedException() { AggregateType = sourceType };
        }
    }
}
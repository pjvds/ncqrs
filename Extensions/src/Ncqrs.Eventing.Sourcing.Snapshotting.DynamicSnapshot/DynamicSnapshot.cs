using System;
using System.Linq;
using System.Reflection;
using Ncqrs.Domain;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{
    public static class DynamicSnapshot
    {

        #region Fields

        [ThreadStatic]
        private static Assembly _snapshotAssembly;

        #endregion

        #region Public Methods

        public static Assembly CreateAssemblyFrom(Assembly target)
        {
            var snapshotableTypes = target.GetTypes().Where(type => type.HasAttribute(typeof(DynamicSnapshotAttribute)));

            if (snapshotableTypes.Count() == 0)
                throw new DynamicSnapshotException(string.Format(
                    "The assembly '{}' does not contain any snapshotable types. Are you missing [{1}] attribute?",
                    target.FullName,
                    typeof(DynamicSnapshotAttribute).Name));

            var snapshotAssemblyBuilder = new DynamicSnapshotAssemblyBuilder();

            foreach (var type in snapshotableTypes)
                snapshotAssemblyBuilder.RegisterSnapshotType(type);

            _snapshotAssembly = snapshotAssemblyBuilder.SaveAssembly();

            return _snapshotAssembly;
        }

        public static void InitializeFrom(DynamicSnapshotBase snapshot, AggregateRoot source)
        {
            if (snapshot == null) throw new ArgumentNullException("snapshot");
            if (source == null) throw new ArgumentNullException("source");

            var sourceFieldMap = SnapshotableField.GetMap(source.GetType());
            var snapshotFields = snapshot.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

            foreach (var snapshotField in snapshotFields)
            {
                FieldInfo sourceField;
                if (sourceFieldMap.TryGetValue(snapshotField.Name, out sourceField))
                {
                    var fieldValue = sourceField.GetValue(source);
                    snapshotField.SetValue(snapshot, fieldValue);
                }
                else
                {
                    // TODO: No field found; throw?
                }
            }
        }

        public static void RestoreAggregateRoot(DynamicSnapshotBase snapshot, AggregateRoot source)
        {
            if (snapshot == null) throw new ArgumentNullException("snapshot");
            if (source == null) throw new ArgumentNullException("source");

            var arFieldMap = SnapshotableField.GetMap(source.GetType());
            var snapshotFields = snapshot.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

            foreach (var snapshotField in snapshotFields)
            {
                FieldInfo arField;
                if (arFieldMap.TryGetValue(snapshotField.Name, out arField))
                {
                    var fieldValue = snapshotField.GetValue(snapshot);
                    arField.SetValue(source, fieldValue);
                }
                else
                {
                    // TODO: No field found; throw?
                }
            }
        }

        #endregion

        #region Internal Methods

        internal static Assembly LoadSnapshotAssembly()
        {
            if (_snapshotAssembly == null)
            {
                try
                {
                    _snapshotAssembly = Assembly.LoadFrom(DynamicSnapshotAssemblyBuilder.DefaultModuleName);
                }
                catch (Exception ex)
                {
                    throw new DynamicSnapshotException("See inner exception for details.", ex);
                }
            }
            return _snapshotAssembly;
        }

        #endregion

        #region Private Methods

        public static Type FindSnapshotType<T>(T aggregate) where T : AggregateRoot
        {
            return FindSnapshotType<T>();
        }

        public static Type FindSnapshotType<T>() where T : AggregateRoot 
        {
            LoadSnapshotAssembly();
            var aggregateTypeName = typeof(T).Name;
            var snapshotType = _snapshotAssembly.GetTypes().SingleOrDefault(type => type.Name.StartsWith(aggregateTypeName));

            if (snapshotType == null)
                throw new DynamicSnapshotException(string.Format(
                    "Cannot find snapshot in '{0}' for type [{1}]. Consider rebuilding the dynamic snapshot assembly.",
                    _snapshotAssembly.FullName,
                    aggregateTypeName));

            return snapshotType;
        }

        #endregion

    }
}
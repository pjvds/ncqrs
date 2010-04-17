using System;
using StructureMap;

namespace Ncqrs.Config.StructureMap
{
    /// <summary>
    /// A environment configuration based on structure map. All requested instances
    /// will be get from the structure map <see cref="ObjectFactory"/>.
    /// </summary>
    public class StructureMapConfiguration : IEnvironmentConfiguration
    {
        public StructureMapConfiguration(Action<IInitializationExpression> initialization = null, Action<ConfigurationExpression> configuration = null)
        {
            if (initialization != null)
            {
                ObjectFactory.Initialize(initialization);
            }
            if (configuration != null)
            {
                ObjectFactory.Configure(configuration);
            }
        }

        public T Get<T>() where T : class
        {
            return ObjectFactory.GetInstance<T>();
        }


        public bool TryGet<T>(out T result) where T : class
        {
            result = default(T);
            var foundInstance = ObjectFactory.TryGetInstance(typeof(T));

            if (foundInstance != null)
            {
                result = (T)foundInstance;
            }

            return result != null;
        }
    }
}

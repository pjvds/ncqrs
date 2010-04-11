using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap;

namespace Ncqrs.Config.StructureMap
{
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

        public T Get<T>()
        {
            return ObjectFactory.GetInstance<T>();
        }


        public bool TryGet<T>(out T result)
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

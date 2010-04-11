using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Eventing.Bus;
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
    }
}

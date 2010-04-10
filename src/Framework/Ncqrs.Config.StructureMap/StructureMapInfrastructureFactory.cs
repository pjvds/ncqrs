using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Eventing.Bus;
using StructureMap;

namespace Ncqrs.Config.StructureMap
{
    public class StructureMapConfiguration : IConfigurationSource
    {
        public StructureMapConfiguration(Action<IInitializationExpression> initialization = null, Action<global::StructureMap.ConfigurationExpression> configuration = null)
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

        public IEventBusFactory CreateEventBusFactory()
        {
            IEventBus eventBus = ObjectFactory.GetInstance<IEventBus>();
        
            return new MethodBasedEventBusFactory(()=>eventBus);
        }
    }
}

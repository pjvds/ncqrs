using System;
using System.Linq;
using Ncqrs.Config;
using Castle.Windsor;

namespace Ncqrs.RhinoServiceBus
{
    public class RsbEnvironmentConfiguration : IEnvironmentConfiguration
    {
        private readonly IWindsorContainer _container;

        public RsbEnvironmentConfiguration(IWindsorContainer container)
        {
            _container = container;
        }

        public bool TryGet<T>(out T result) where T : class
        {
            var built = _container.ResolveAll<T>();
            if (built.Count() == 1)
            {
                result = built.First();
                return true;
            }
            if (built.Count() == 0)
            {
                result = null;
                return false;
            }
            throw new InvalidOperationException("More than one implementation of requested type " + typeof(T).FullName);
        }
    }
}

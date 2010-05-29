using System;
using System.Linq;
using Ncqrs.Config;
using NServiceBus.ObjectBuilder;

namespace Ncqrs.NServiceBus
{
    /// <summary>
    /// Ncqrs environment configuration integrated with NServiceBus DI infrastructe
    /// abstractions. Pulls objects from NServiceBus bult-in container.
    /// </summary>
    public class NsbEnvironmentConfiguration : IEnvironmentConfiguration
    {
        private readonly IBuilder _builder;

        public NsbEnvironmentConfiguration(IBuilder builder)
        {
            _builder = builder;
        }

        public bool TryGet<T>(out T result) where T : class
        {
            var built = _builder.BuildAll<T>();
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
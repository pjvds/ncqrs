using System;
using System.Collections.Generic;
using Ncqrs.Config;

namespace Ncqrs.Spec.Fakes
{

    public class EnvironmentConfigurationWrapper : IEnvironmentConfiguration 
    {
        private readonly IEnvironmentConfiguration _configuration;
        private readonly Dictionary<Type, Func<object>> _factories;

        public EnvironmentConfigurationWrapper()
            : this(NcqrsEnvironment.CurrentConfiguration)
        {
        }

        public EnvironmentConfigurationWrapper(IEnvironmentConfiguration configuration)
        {
            _configuration = configuration;
            _factories = new Dictionary<Type, Func<object>>();
        }

        public bool TryGet<T>(out T result) where T : class
        {
            result = null;
            var type = typeof(T);
            Func<object> factory;
            if (_factories.TryGetValue(type, out factory))
            {
                result = (T) factory();
                return true;
            }
            return _configuration != null && _configuration.TryGet(out result);
        }

        public void Register<T>(T singleton)
        {
            Register(() => singleton);
        }

        public void Register<T>(Func<T> factory)
        {
            _factories[typeof (T)] = () => factory();
        }

        public bool Unregister<T>()
        {
            if (_factories.ContainsKey(typeof(T)))
            {
                _factories.Remove(typeof (T));
                return true;
            }
            return false;
        }

        public void Push()
        {
            if (NcqrsEnvironment.CurrentConfiguration != _configuration)
                throw new Exception("This EnvironmentConfigurationWrapper doesn't wrap the current configuration.");
            if (NcqrsEnvironment.IsConfigured)
                NcqrsEnvironment.Deconfigure();
            NcqrsEnvironment.Configure(this);
        }

        public void Pop()
        {
            if (NcqrsEnvironment.CurrentConfiguration != this)
                throw new Exception("The current configuration isn't this instance of EnvironmentConfigurationWrapper");
            NcqrsEnvironment.Deconfigure();
            if (_configuration != null)
                NcqrsEnvironment.Configure(_configuration);
        }

    }

}

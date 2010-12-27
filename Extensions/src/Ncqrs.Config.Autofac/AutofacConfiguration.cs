using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;

namespace Ncqrs.Config.Autofac
{
    public class AutofacConfiguration : IEnvironmentConfiguration
    {
        private readonly IContainer _container;

        public AutofacConfiguration(IContainer container)
        {
            _container = container;
        }

        public bool TryGet<T>(out T result) where T : class
        {
            if (!_container.TryResolve<T>(out result))
            {
                result = default(T);
            }

            return result != null;
        }
    }
}

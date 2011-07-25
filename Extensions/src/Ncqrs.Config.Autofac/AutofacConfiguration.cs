using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;

namespace Ncqrs.Config.Autofac
{
    /// <summary>
    /// A environment configuration based on Autofac. All requested instances
    /// will be get from the Autofac <see cref="IKernel"/>.
    /// </summary>
    /// <code>
    /// var kernel = new Ninject.StandardKernel(new NcqrsModule());
    /// var config = new NinjectConfiguration(kernel);
    /// NcqrsEnvironment.Configure(config);
    /// </code>
    public class AutofacConfiguration : IEnvironmentConfiguration
    {

        private readonly IContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacConfiguration" /> class.
        /// </summary>
        /// <param name="kernel">The Autofac container which will provide components to Ncqrs</param>
        public AutofacConfiguration(IContainer container)
        {
            _container = container;
        }

        public bool TryGet<T>(out T result) where T : class
        {
            return _container.TryResolve<T>(out result);
        }
    }
}

using Ninject;

namespace Ncqrs.Config.Ninject
{

    /// <summary>
    /// A environment configuration based on Ninject. All requested instances
    /// will be get from the Ninject <see cref="IKernel"/>.
    /// </summary>
    /// <code>
    /// var kernel = new Ninject.StandardKernel(new NcqrsModule());
    /// var config = new NinjectConfiguration(kernel);
    /// NcqrsEnvironment.Configure(config);
    /// </code>
    public class NinjectConfiguration : IEnvironmentConfiguration
    {

        private readonly IKernel _kernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="NinjectConfiguration" /> class.
        /// </summary>
        /// <param name="kernel">The Ninject kernel which will provide components to Ncqrs</param>
        public NinjectConfiguration(IKernel kernel)
        {
            _kernel = kernel;
        }

        public bool TryGet<T>(out T result) where T : class
        {
            result = _kernel.TryGet<T>();
            return result != null;
        }
    }

}

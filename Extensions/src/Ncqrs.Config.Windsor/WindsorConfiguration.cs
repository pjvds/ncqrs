using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Ncqrs.Config.Windsor
{
    /// <summary>
    ///  A environment configuration based on Castle Windsor.
    /// </summary>
    /// <code>
    /// var container = new WindsorContainer();
    /// // ... configure container
    /// NcqrsEnvironment.Configure(new WindsorConfiguration(container));
    /// </code>
    public class WindsorConfiguration : IEnvironmentConfiguration
    {
        readonly IWindsorContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindsorConfiguration"/> class.
        /// </summary>
        /// <param name="container">The Windsor container which will provide components to Ncqrs.</param>
        public WindsorConfiguration(IWindsorContainer container)
        {
            this.container = container;
            container.Register(Component.For<IWindsorContainer>().Instance(container));
        }

        /// <summary>
        /// Tries to get the specified instance.
        /// </summary>
        /// <typeparam name="T">The type of the instance to get.</typeparam>
        /// <param name="result">The result.</param>
        /// <returns>A indication whether the instance could be get or not.</returns>
        public bool TryGet<T>(out T result) where T : class
        {
            result = container.Kernel.HasComponent(typeof(T)) ? container.Resolve<T>() : default(T);

            return result != null;
        }
    }
}

using Autofac;

namespace Ncqrs.Config.Autofac
{
    /// <summary>
    ///  A environment configuration based on Autofac.
    /// </summary>
    /// <code>
    /// var builder = new ContainerBuilder();
    /// // ... configure container
    /// NcqrsEnvironment.Configure(new AutofacConfiguration(builder.Build()));
    /// </code>
    public class AutofacConfiguration : IEnvironmentConfiguration
    {
        private readonly IContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacConfiguration"/> class.
        /// </summary>
        /// <param name="container">The Autofac container which will provide components to Ncqrs.</param>
        public AutofacConfiguration(IContainer container)
        {
            _container = container;
        }

        #region IEnvironmentConfiguration Members

        /// <summary>
        /// Tries to get the specified instance.
        /// </summary>
        /// <typeparam name="T">The type of the instance to get.</typeparam>
        /// <param name="result">The result.</param>
        /// <returns>An indication whether the instance could be get or not.</returns>
        public bool TryGet<T>(out T result) where T : class
        {
            if (!_container.TryResolve(out result))
            {
                result = default(T);
            }

            return result != default(T);
        }

        #endregion
    }
}
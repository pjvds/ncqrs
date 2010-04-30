using System;
using StructureMap;

namespace Ncqrs.Config.StructureMap
{
    /// <summary>
    /// A environment configuration based on structure map. All requested instances
    /// will be get from the structure map <see cref="ObjectFactory"/>.
    /// </summary>
    /// <code>
    /// var config = new StructureMapConfiguration(x =&gt;
    /// {
    /// var eventBus = InitializeEventBus();
    /// var eventStore = InitializeEventStore();
    /// x.For{IEventBus}().Use(eventBus);
    /// x.For{IEventStore}().Use(eventStore);
    /// x.For{IUnitOfWorkFactory}().Use{UnitOfWorkFactory}();
    /// });
    /// NcqrsEnvironment.Configure(config);
    /// </code>
    public class StructureMapConfiguration : IEnvironmentConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StructureMapConfiguration"/> class.
        /// </summary>
        /// <param name="initialization">The initialization expression.
        /// <remarks>If this is not <c>null</c> the <c>ObjectFactory.Initialize</c> method will be called it this expression.</remarks></param>
        /// <param name="configuration">The configuration expression.
        ///  <remarks>If this is not <c>null</c> the <c>ObjectFactory.Configure</c> method will be called it this expression.</remarks></param>
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

        /// <summary>
        /// Tries to get the specified instance.
        /// </summary>
        /// <typeparam name="T">The type of the instance to get.</typeparam>
        /// <param name="result">The result.</param>
        /// <returns>A indication whether the instance could be get or not.</returns>
        public bool TryGet<T>(out T result) where T : class
        {
            result = default(T);
            var foundInstance = ObjectFactory.TryGetInstance<T>();

            if (foundInstance != null)
            {
                result = foundInstance;
            }

            return result != null;
        }
    }
}

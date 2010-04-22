using System;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics.Contracts;
using log4net;
using Ncqrs.Config;
using Ncqrs.Eventing.ServiceModel.Bus;
using Ncqrs.Eventing.Storage;

namespace Ncqrs
{
    /// <summary>
    /// The Ncqrs environment. This class gives access to other components registered in this environment.
    /// <remarks>
    /// Make sure to call the <see cref="Configure"/> method before doing anything else with this class.
    /// </remarks>
    /// </summary>
    public static class NcqrsEnvironment
    {
        static NcqrsEnvironment()
        {
            // Initialize defaults.
            SetDefault<IClock>(new DateTimeBasedClock());
            SetDefault<IUniqueIdentifierGenerator>(new BasicGuidGenerator());
            SetDefault<IEventBus>(new InProcessEventBus());
            SetDefault<IEventStore>(new InMemoryEventStore());
            //TODO: Added IDomainRepository default..
        }

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Holds the defaults for requested types that are not configured.
        /// </summary>
        /// <remarks>
        /// Use the <see cref="SetDefault{T}"/> method to set a default.
        /// </remarks>
        private static readonly Dictionary<Type, Object> _defaults = new Dictionary<Type, object>(0);

        /// <summary>
        /// Hold the environment configuration. This is initialized by the <see cref="Configure"/> method.
        /// </summary>
        private static IEnvironmentConfiguration _instance;

        /// <summary>
        /// Gets or create the requested instance specified by yhe parameter <i>T</i>.
        /// </summary>
        /// <typeparam name="T">The type of the instance that is requested.</typeparam>
        /// <returns>The instance of the requested type specified by <i>T</i>.</returns>
        public static T Get<T>() where T : class
        {
            Log.DebugFormat("Requesting instance {0} from the environment.", typeof(T).FullName);

            T result;

            if (_instance == null || !_instance.TryGet(out result))
            {
                object defaultResult;

                if (!_defaults.TryGetValue(typeof(T), out defaultResult))
                {
                    throw new InstanceNotFoundInEnvironmentConfigurationException(typeof(T));
                }

                result = (T) defaultResult;
            }

            return result;
        }

        /// <summary>
        /// Sets the default for an type. This default instance is returned when
        /// the configured <see cref="IEnvironmentConfiguration"/> did not
        /// returned an instance for this type.
        /// </summary>
        /// <remarks>When the type already contains a default, it is overridden.
        /// </remarks>
        /// <typeparam name="T">The type of the instance to set a default.
        /// </typeparam>
        /// <param name="instance">The instance to set as default.</param>
        public static void SetDefault<T>(T instance) where T : class
        {
            Contract.Requires<ArgumentNullException>(instance != null, "The instance cannot be null.");

            _defaults[typeof (T)] = instance;
        }

        /// <summary>
        /// Removes the default for specified type.
        /// </summary>
        /// <remarks>When there is no default set, this action is ignored.</remarks>
        /// <typeparam name="T">The registered default type.</typeparam>
        public static void RemoveDefault<T>() where T : class
        {
            _defaults.Remove(typeof(T));
        }

        /// <summary>
        /// Configures the Ncqrs environment.
        /// </summary>
        /// <param name="source">The source that contains the configuration for the current environment.</param>
        public static void Configure(IEnvironmentConfiguration source)
        {
            Contract.Requires<ArgumentNullException>(source != null, "The source cannot be null.");
            Contract.Ensures(_instance == source, "The given source should initialize the _instance member.");

            _instance = source;

            Log.InfoFormat("Ncqrs environment configured with {0} configuration source.", source.GetType().FullName);
        }
    }
}

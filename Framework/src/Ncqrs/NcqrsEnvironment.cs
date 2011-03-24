using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution;
using Ncqrs.Config;
using Ncqrs.Domain;
using Ncqrs.Eventing.ServiceModel.Bus;
using Ncqrs.Eventing.Sourcing.Snapshotting;
using Ncqrs.Eventing.Storage;
using Ncqrs.Domain.Storage;

namespace Ncqrs
{
    /// <summary>The Ncqrs environment. This class gives access to other components registered in this environment.
    /// <remarks>
    /// Make sure to call the <see cref="Configure"/> method before doing anything else with this class.
    /// </remarks></summary>
    public static class NcqrsEnvironment
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static NcqrsEnvironment()
        {
            InitDefaults();
        }

        /// <summary>
        /// Initialize defaults with default components.
        /// </summary>
        private static void InitDefaults()
        {
            // Initialize defaults.
            SetDefault<IClock>(new DateTimeBasedClock());
            SetDefault<IUniqueIdentifierGenerator>(new BasicGuidGenerator());
            SetDefault<IEventBus>(new InProcessEventBus());
            SetDefault<IEventStore>(new InMemoryEventStore());
            SetDefault<ISnapshotStore>(new NullSnapshotStore());
            SetDefault<IUnitOfWorkFactory>(new UnitOfWorkFactory());
            SetDefault<IKnownCommandsEnumerator>(new AllCommandsInAppDomainEnumerator());
            SetDefault<ITransactionService>(new DefaultTransactionService());
            SetDefault<ISnapshottingPolicy>(new NoSnapshottingPolicy());
            SetDefault<IAggregateRootCreationStrategy>(new SimpleAggregateRootCreationStrategy());
            SetDefault<IAggregateSupportsSnapshotValidator>(new AggregateSupportsSnapshotValidator());
            SetDefault<IAggregateSnapshotter>(new DefaultAggregateSnapshotter(Get<IAggregateRootCreationStrategy>(), Get<IAggregateSupportsSnapshotValidator>()));
        }

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
        /// Gets or create the requested instance specified by the parameter <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>This</remarks>
        /// <typeparam name="T">The type of the instance that is requested.
        /// </typeparam>
        /// <returns>If the type specified by <typeparamref name="T"/> is
        /// registered, it returns an instance that is, is a super class of, or
        /// implements the type specified by <typeparamref name="T"/>. Otherwise
        /// a <see cref="InstanceNotFoundInEnvironmentConfigurationException"/>
        /// occurs.
        /// </returns>
        public static T Get<T>() where T : class
        {
            Contract.Ensures(Contract.Result<T>() != null, "The result cannot be null.");

            Log.DebugFormat("Requesting instance {0} from the environment.", typeof(T).FullName);

            T result = null;

            if (_instance == null || !_instance.TryGet(out result))
            {
                object defaultResult;

                if (_defaults.TryGetValue(typeof(T), out defaultResult))
                {
                    result = (T)defaultResult;
                    
                }
            }

            if(result == null)
                throw new InstanceNotFoundInEnvironmentConfigurationException(typeof(T));

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

            _defaults[typeof(T)] = instance;
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
            Contract.Requires<InvalidOperationException>(!IsConfigured, "Cannot configure the environment when it is already configured.");
            Contract.Ensures(_instance == source, "The given source should initialize the _instance member.");
            Contract.Ensures(IsConfigured, "The given source should configure this environment.");

            _instance = source;

            Log.InfoFormat("Ncqrs environment configured with {0} configuration source.", source.GetType().FullName);
        }

        /// <summary>
        /// When the environment is configured it removes the configuration. Defaults are not removed.
        /// </summary>
        public static void Deconfigure()
        {
            _instance = null;
            _defaults.Clear();

            InitDefaults();
        }

        /// <summary>
        /// Gets a value indicating whether this environment is configured.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this environment is configured; otherwise, <c>false</c>.
        /// </value>
        public static Boolean IsConfigured
        {
            get
            {
                return _instance != null;
            }
        }

        /// <summary>
        /// Returns the current environment configuration
        /// </summary>
        /// <remarks>
        /// Returns the current environment configuration, or null if not configured
        /// </remarks>
        public static IEnvironmentConfiguration CurrentConfiguration { get { return _instance; } }

    }
}
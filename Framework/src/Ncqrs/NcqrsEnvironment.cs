using System;
using System.Reflection;
using System.Diagnostics.Contracts;
using log4net;
using Ncqrs.Config;

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
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Hold the environment configuration. This is initialized by the <see cref="Configure"/> method.
        /// </summary>
        private static IEnvironmentConfiguration _instance;

        /// <summary>
        /// Gets or create the requested instance specified by yhe parameter <i>T</i>.
        /// </summary>
        /// <typeparam name="T">The type of the instance that is requested.</typeparam>
        /// <exception cref="EnvironmentNotConfiguredException">Occurs when the environment is not configured. Call <see cref="Configure"/> to configure
        ///   the environment before requesting an instance.</exception>
        /// <returns>The instance of the requested type specified by <i>T</i>.</returns>
        public static T Get<T>() where T : class
        {
            if(_instance == null) throw new EnvironmentNotConfiguredException("The Ncqrs environment is not configured. Use the Configure method to configure it first.");

            Log.DebugFormat("Requesting instance {0} from the environment.", typeof(T).FullName);

            T result;

            if (!_instance.TryGet(out result))
            {
                throw new InstanceNotFoundInEnvironmentConfigurationException(typeof (T));
            }

            return result;
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

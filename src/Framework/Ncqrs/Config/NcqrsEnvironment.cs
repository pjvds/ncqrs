using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Diagnostics.Contracts;

namespace Ncqrs.Config
{
    /// <summary>
    /// The Ncqrs environment. This class gives access to other components registered in this environment.
    /// <remarks>
    /// Make sure to call the <see cref="Configure"/> method before doing anything else with this class.
    /// </remarks>
    /// </summary>
    public class NcqrsEnvironment
    {
        private static IEnvironmentConfiguration _instance;

        /// <summary>
        /// Gets or create the requested instance specified by yhe parameter <i>T</i>.
        /// </summary>
        /// <typeparam name="T">The type of the instance that is requested.</typeparam>
        /// <returns>The instance of the requested type specified by <i>T</i>.</returns>
        public static T Get<T>() where T : class
        {
            return _instance.Get<T>();
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
        }
    }
}

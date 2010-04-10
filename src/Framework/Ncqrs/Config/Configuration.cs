using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Eventing.Bus;
using System.Configuration;
using System.Diagnostics.Contracts;

namespace Ncqrs.Config
{
    public class Configuration
    {
        private static IConfigurationSource _instance;

        public IConfigurationSource Instance
        {
            get
            {
                Contract.Requires<InvalidOperationException>(_instance == null, "The configuration is not configured. Call the Configure method first.");

                return _instance;
            }
        }

        public static void Configure(IConfigurationSource source)
        {
            Contract.Requires<ArgumentNullException>(source != null, "The source cannot be null.");

            _instance = source;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Eventing.Bus;
using System.Configuration;
using System.Diagnostics.Contracts;

namespace Ncqrs.Config
{
    public class NcqrsEnvironment
    {
        private static IEnvironmentConfiguration _instance;

        public static T Get<T>()
        {
            return _instance.Get<T>();
        }

        public IEnvironmentConfiguration Instance
        {
            get
            {
                Contract.Requires<InvalidOperationException>(_instance == null, "The configuration is not configured. Call the Configure method first.");

                return _instance;
            }
        }

        public static void Configure(IEnvironmentConfiguration source)
        {
            Contract.Requires<ArgumentNullException>(source != null, "The source cannot be null.");

            _instance = source;
        }
    }
}

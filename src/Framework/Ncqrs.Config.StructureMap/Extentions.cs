using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap;

namespace Ncqrs.Config.StructureMap
{
    public static class Extentions
    {
        public static void ConfigureWithStructureMap(this Configuration target, Action<IInitializationExpression> initialization, Action<ConfigurationExpression> configuration)
        {
            // TODO: validate.
            if (target != null) throw new InvalidOperationException();

            Configuration.Configure(new StructureMapConfiguration(initialization, configuration));
        }
    }
}

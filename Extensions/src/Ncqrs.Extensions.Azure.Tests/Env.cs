using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Eventing.Storage;
using Ncqrs.Extensions.Azure.Storage;
using Microsoft.ApplicationServer.Caching;
using Ncqrs.Domain;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;
using Ncqrs.Extensions.Azure.Domain;
using Ncqrs.Commanding.ServiceModel;



namespace Ncqrs.Extensions.Azure.Tests
{
    public static class Env
    {
        public static string ConfigureTestEnvironment(bool useTableStorage, bool useAppFabric)
        {
            NcqrsEnvironment.Deconfigure();
            string TestPrefix = "Environment" + DateTime.Now.ToString("yyyymmddHHmmss");
            if (useTableStorage)
            {
                NcqrsEnvironment.SetDefault<IEventStore>(new TableOnlyStore(TestPrefix));
            }
            else
            {
                NcqrsEnvironment.SetDefault<IEventStore>(new InMemoryEventStore());
            }

            if (useAppFabric)
            {

                DataCacheFactoryConfiguration config = new DataCacheFactoryConfiguration();
                config.SecurityProperties = new DataCacheSecurity(DataCacheSecurityMode.None, DataCacheProtectionLevel.None);
                DataCacheServerEndpoint ep = new DataCacheServerEndpoint("localhost", 22233);
                List<DataCacheServerEndpoint> endPoints = new List<DataCacheServerEndpoint>();
                endPoints.Add(ep);
                config.Servers = endPoints;

                NcqrsEnvironment.SetDefault<IUnitOfWorkFactory>(new CachedUnitOfWorkFactory(config));
            }

            CommandService c = new CommandService();
            c.RegisterExecutorsInAssembly(typeof(Env).Assembly);
            NcqrsEnvironment.SetDefault<ICommandService>(c);

            return TestPrefix;
        }
    }
}

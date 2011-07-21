using System;
using Rhino.ServiceBus.Hosting;
using Castle.Core.Configuration;
using Ncqrs.Commanding.ServiceModel;
using Ncqrs.Commanding.CommandExecution;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;
using Ncqrs.Commanding.CommandExecution.Mapping;
using Ncqrs.Commanding;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Rhino.ServiceBus.Config;
using Rhino.ServiceBus.Impl;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel;

namespace Ncqrs.RhinoServiceBus
{
    public class NcqrsBootStrapper : AbstractBootStrapper
    {
        private string _commandService;

        public override void AfterStart()
        {
            base.AfterStart();

            NcqrsEnvironment.Configure(new RsbEnvironmentConfiguration(container));
        }
    }


    public class Ncqrs : IBusConfigurationAware
    {

        public void Configure(AbstractRhinoServiceBusFacility facility, IConfiguration configuration)
        {
            throw new NotImplementedException();
        }
    }

    public class NcqrsConfiguration
    {
        IWindsorContainer _container = new WindsorContainer();
        
        public NcqrsConfiguration InstallNcqrs(IWindsorContainer container)
        {
            
            _commandService = new RsbCommandService();
            container.Register(
                Component
                    .For<ICommandService>()
                    .ImplementedBy<RsbCommandService>()
                    .Instance(_commandService)
                    .LifeStyle.Singleton);

            return this;
        }

        private RsbCommandService _commandService;

        public NcqrsConfiguration RegisterExecutorsForMappedCommandsInAssembly(System.Reflection.Assembly assembly)
        {
            _commandService.RegisterExecutorsInAssembly(assembly);
            return this;
        }
    }
}

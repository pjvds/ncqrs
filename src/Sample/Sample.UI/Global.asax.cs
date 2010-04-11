using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ncqrs.CommandExecution;
using Ncqrs.Eventing.Bus;
using Sample.Commands;
using Ncqrs.CommandExecution.AutoMapping;
using Ncqrs.Domain;
using Ncqrs.Eventing.Storage.MongoDB;
using MongoDB.Driver;
using System.Threading;
using Sample.Events;
using Sample.ReadModel.Denormalizers;
using Sample.UI.Controllers;
using Sample.ReadModel.Denormalizers.EditMessageModel;
using Sample.ReadModel.Denormalizers.MessageModel;
using Ncqrs.Denormalization;
using System.Reflection;
using log4net;
using log4net.Config;
using Ncqrs.Config;
using Ncqrs.Config.StructureMap;
using Ncqrs.Eventing.Storage;

namespace Sample.UI
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static ICommandExecutor CommandExecutor
        {
            get
            {
                return (ICommandExecutor)HttpContext.Current.Application["CommandExecutor"];
            }
            private set
            {
                HttpContext.Current.Application["CommandExecutor"] = value;
            }
        }

        public static IEventBus EventBus
        {
            get
            {
                return (IEventBus)HttpContext.Current.Application["EventBus"];
            }
            private set
            {
                HttpContext.Current.Application["EventBus"] = value;
            }
        }


        public override void Init()
        {
            var config = new StructureMapConfiguration(x =>
                {
                    var eventBus = InitializeEventBus();
                    var eventStore = InitializeEventStore();

                    x.ForRequestedType<IEventBus>().TheDefault.IsThis(eventBus);
                    x.ForRequestedType<IEventStore>().TheDefault.IsThis(eventStore);
                    x.ForRequestedType<IUnitOfWorkFactory>().TheDefaultIsConcreteType<ThreadBasedUnitOfWorkFactory>();
                });

            NcqrsEnvironment.Configure(config);
            InitializeCommandService();

            // Configure log4net.
            XmlConfigurator.Configure();

            this.Error += new EventHandler(MvcApplication_Error);
        }

        void MvcApplication_Error(object sender, EventArgs e)
        {
            Session["Error"] = Server.GetLastError();

            this.Response.Redirect("/Home/Error");
        }

        private ICommandExecutor InitializeCommandService()
        {
            var commandService = new InProcessCommandExecutionDispatcher();
            commandService.RegisterExecutor<AddNewMessageCommand>(new AutoMappingCommandExecutor());
            commandService.RegisterExecutor<UpdateMessageTextCommand>(new AutoMappingCommandExecutor());

            CommandExecutor = commandService;

            return CommandExecutor;
        }

        private IEventBus InitializeEventBus()
        {
            EventBus = new InProcessEventBus();

            var factory = new DenormalizerFactory();
            var denormalizers = factory.CreateDenormalizersFromAssembly(typeof(EditMessageModelMessageTextUpdatedDenormalizer).Assembly);

            foreach (var denormalizer in denormalizers)
            {
                EventBus.RegisterHandler(denormalizer);
            }

            return EventBus;
        }

        private IEventStore InitializeEventStore()
        {
            var eventStore = new MongoDBEventStore(new Mongo());
            return eventStore;
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default",                                              // Route name
                "{controller}/{action}/{id}",                           // URL with parameters
                new { controller = "Home", action = "Welcome", id = "" }  // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);
        }
    }
}
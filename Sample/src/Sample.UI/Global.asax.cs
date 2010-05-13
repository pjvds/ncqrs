using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ncqrs;
using Ncqrs.Commanding.CommandExecution;
using Ncqrs.Config.StructureMap;
using Ncqrs.Eventing.Denormalization;
using Ncqrs.Eventing.ServiceModel.Bus;
using Ncqrs.Eventing.Storage;
using Ncqrs.Eventing.Storage.MongoDB;
using Sample.Commands;
using Ncqrs.Domain;
using Sample.ReadModel.Denormalizers.EditMessageModel;

namespace Sample.UI
{
    public class MvcApplication : HttpApplication
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
            var eventStore = new MongoDBEventStore();
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
            var config = new StructureMapConfiguration(x =>
            {
                var eventBus = InitializeEventBus();
                var eventStore = InitializeEventStore();

                x.For<IEventBus>().Use(eventBus);
                x.For<IEventStore>().Use(eventStore);
                x.For<IUnitOfWorkFactory>().Use<ThreadBasedUnitOfWorkFactory>();
            });

            NcqrsEnvironment.Configure(config);
            InitializeCommandService();

            // Configure log4net.
            log4net.Config.XmlConfigurator.Configure();

            this.Error += new EventHandler(MvcApplication_Error);

            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);
        }
    }
}
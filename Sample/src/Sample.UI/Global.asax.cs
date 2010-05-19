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
using Ncqrs.Eventing.Storage.SQL;
using Ncqrs.Domain.Storage;

namespace Sample.UI
{
    public class MvcApplication : HttpApplication
    {
        public static ICommandService CommandExecutor
        {
            get
            {
                return (ICommandService)HttpContext.Current.Application["CommandExecutor"];
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

        private ICommandService InitializeCommandService()
        {
            var commandService = new InProcessCommandService();
            commandService.RegisterExecutor(new MappedCommandExecutor<AddNewMessageCommand>());
            commandService.RegisterExecutor(new MappedCommandExecutor<UpdateMessageTextCommand>());

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
            return new SimpleMicrosoftSqlServerEventStore(@"Data Source=PCZWO_006\SQLEXPRESS_2008;Initial Catalog=ncqrs;User Id=sa;Password=sigma;");
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
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
                x.For<IDomainRepository>().Use(new DomainRepository(eventStore, eventBus));
                x.For<IUnitOfWorkFactory>().Use<UnitOfWorkFactory>();
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
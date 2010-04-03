using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ncqrs.CommandHandling;
using Ncqrs.Eventing.Bus;
using Sample.Commands;
using Ncqrs.CommandHandling.AutoMapping;
using Ncqrs.Domain;
using Ncqrs.Eventing.Storage.MongoDB;
using MongoDB.Driver;
using System.Threading;
using Sample.Events;
using Sample.ReadModel.Denormalizers;
using Sample.UI.Controllers;

namespace Sample.UI
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static ICommandService CommandService
        {
            get
            {
                return (ICommandService)HttpContext.Current.Application["CommandService"];
            }
            private set
            {
                HttpContext.Current.Application["CommandService"] = value;
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
            InitializeEventBus();
            InitializeCommandService();

            this.Error += new EventHandler(MvcApplication_Error);
        }

        void MvcApplication_Error(object sender, EventArgs e)
        {
            Session["Error"] = Server.GetLastError();

            this.Response.Redirect("/Home/Error");
        }

        private void InitializeCommandService()
        {
            var eventStore = new MongoDBEventStore(new Mongo());
            var repository = new DomainRepository(eventStore, EventBus);

            CommandService = new TransactionalInProcessCommandService();
            CommandService.RegisterHandler<AddNewMessageCommand>(new AutoMappingCommandHandler<AddNewMessageCommand>(repository));
        }

        private void InitializeEventBus()
        {
            EventBus = new InProcessEventBus();
            EventBus.RegisterHandler<NewMessageAdded>(new NewMessageDenormalizer());
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
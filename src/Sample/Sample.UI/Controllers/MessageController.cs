using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sample.ReadModel;
using Sample.Commands;
using Ncqrs.CommandHandling;

namespace Sample.UI.Controllers
{
    public class MessageController : Controller
    {
        //
        // GET: /Message/

        public ActionResult Index()
        {
            var repository = new ReadRepository<MessageModel>();
            var model = repository.FindAll();

            return View();
        }

        public ActionResult Add()
        {
            return View(new AddNewMessageCommand());
        }

        [HttpPost]
        public ActionResult Add(AddNewMessageCommand command)
        {
            ICommandService service = MvcApplication.CommandService;
            service.Execute(command);

            return View();
        }
    }
}

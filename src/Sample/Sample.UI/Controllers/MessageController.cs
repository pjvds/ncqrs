using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sample.ReadModel;
using Sample.Commands;
using Ncqrs.CommandHandling;
using MongoDB.Driver;

namespace Sample.UI.Controllers
{
    public class MessageController : Controller
    {
        public ActionResult Index()
        {
            var repository = new ReadRepository<MessageModel>();
            var model = repository.Find(new Document().Append("query", new Document()).Append("orderby", new Document().Append("CreationDate", -1)));

            if (model.Count() == 0)
                return RedirectToAction("NoMessageFound");
            else
                return View(model);
        }

        public ActionResult Add()
        {
            return View(new AddNewMessageCommand());
        }

        public ActionResult NoMessageFound()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(AddNewMessageCommand command)
        {
            ICommandService service = MvcApplication.CommandService;
            service.Execute(command);

            return RedirectToAction("Index");
        }
    }
}

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
            var repository = new ReadRepository<IMessageModel>();
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

        public ActionResult Edit()
        {
            var messageId = Guid.Parse(Request.QueryString["MessageId"]);

            var repository = new ReadRepository<IEditMessageModel>();
            var model = repository.Find(new Document().Append("Id", messageId)).First();

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Guid messageId, String text)
        {
            ICommandService service = MvcApplication.CommandService;
            var command = new UpdateMessageTextCommand { MessageId = messageId, NewMessageText = text };
            service.Execute(command);

            return RedirectToAction("Index");
        }
    }
}

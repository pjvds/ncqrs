using System;
using System.Linq;
using System.Web.Mvc;
using Sample.ReadModel;
using Sample.Commands;

namespace Sample.UI.Controllers
{
    public class MessageController : Controller
    {
        public ActionResult Index()
        {
            using (var context = new ReadModelDataContext())
            {
                var messages = (from m in context.MessageModels
                                orderby m.CreationDate
                                select m).ToList();

                if (messages.Count == 0)
                    return RedirectToAction("NoMessageFound");
                else
                    return View(messages);
            }
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
            var service = MvcApplication.CommandExecutor;
            service.Execute(command);

            return RedirectToAction("Index");
        }

        public ActionResult Edit()
        {
            //var messageId = Guid.Parse(Request.QueryString["MessageId"]);

            //using (var repository = new ReadRepository<IEditMessageModel>())
            //{
            //    var model = repository.FindOne(new Document().Append("Id", messageId));

            //    return View(model);
            //}
            throw new NotImplementedException();
        }

        [HttpPost]
        public ActionResult Edit(Guid messageId, String text)
        {
            var service = MvcApplication.CommandExecutor;
            var command = new UpdateMessageTextCommand { MessageId = messageId, NewMessageText = text };
            service.Execute(command);

            return RedirectToAction("Index");
        }
    }
}

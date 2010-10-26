using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Commands;
using ReadModel;
using Website.CommandService;

namespace Website.Controllers
{
    public class NoteController : Controller
    {
        public ActionResult Index()
        {
            IEnumerable<NoteItem> items;

            using (var context = new ReadModelContainer())
            {
                var query = from item in context.NoteItemSet
                            orderby item.CreationDate
                            select item;

                items = query.ToArray();
            }

            return View(items);
        }

        public ActionResult Edit(Guid id)
        {
            NoteItem item;

            using (var context = new ReadModelContainer())
            {
                item = context.NoteItemSet.Single(note => note.Id == id);
            }

            var command = new ChangeNoteText();
            command.NoteId = id;
            command.NewText = item.Text;

            return View(command);
        }

        [HttpPost]
        public ActionResult Edit(ChangeNoteText command)
        {
            var service = new MyNotesCommandServiceClient();
            service.ChangeNoteText(command);

            // Return user back to the index that
            // displays all the notes.));
            return RedirectToAction("Index");
        }

        public ActionResult Add()
        {
            var command = new CreateNewNote();
            command.NoteId = Guid.NewGuid();

            return View(command);
        }

        [HttpPost]
        public ActionResult Add(CreateNewNote command)
        {
            var service = new MyNotesCommandServiceClient();
            service.CreateNewNote(command);

            // Return user back to the index that
            // displays all the notes.));
            return RedirectToAction("Index");
        }

        public ActionResult Report()
        {
            IEnumerable<TotalsPerDayItem> items;

            using (var context = new ReadModelContainer())
            {
                var query = from item in context.TotalsPerDayItemSet
                            orderby item.Date descending
                            select item;

                items = query.ToArray();
            }

            return View(items);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Mvc;

using Ncqrs.CommandService;
using Ncqrs.CommandService.Contracts;

using MyNotes.Commands;
using MyNotes.ReadModel.Context;
using MyNotes.ReadModel.Types;

namespace MyNotes.Web.MVC4.Controllers
{
    public class NoteController : Controller
    {
        private static ChannelFactory<ICommandWebServiceClient> channelFactory;

        static NoteController()
        {
            NoteController.channelFactory = new ChannelFactory<ICommandWebServiceClient>("CommandWebServiceClient");
        }

        public ActionResult Index()
        {
            IEnumerable<Note> notes;

            using (var context = new ReadModelContext())
            {
                var query = from item in context.Notes
                            orderby item.CreationDate
                            select item;

                notes = query.ToArray();
            }

            return View(notes);
        }

        public ActionResult Edit(Guid id)
        {
            Note note;

            using (var context = new ReadModelContext())
            {
                note = context.Notes.Single(n => n.Id == id);
            }

            var command = new ChangeNoteText();
            command.Id = id;
            command.Text = note.Text;

            return View(command);
        }

        [HttpPost]
        public ActionResult Edit(ChangeNoteText command)
        {
            ChannelHelper.Use(NoteController.channelFactory.CreateChannel(), (client) => client.Execute(new ExecuteRequest(command)));

            // Return user back to the index that
            // displays all the notes.));
            return RedirectToAction("Index", "Note");
        }

        public ActionResult Add()
        {
            var command = new CreateNewNote();
            command.Id = Guid.NewGuid();

            return View(command);
        }

        [HttpPost]
        public ActionResult Add(CreateNewNote command)
        {
            ChannelHelper.Use(NoteController.channelFactory.CreateChannel(), (client) => client.Execute(new ExecuteRequest(command)));

            // Return user back to the index that
            // displays all the notes.));
            return RedirectToAction("Index", "Note");
        }

        public ActionResult Report()
        {
            IEnumerable<DailyStatistics> dailyStatistics;

            using (var context = new ReadModelContext())
            {
                var query = from item in context.DailyStatistics
                            orderby item.Date descending
                            select item;

                dailyStatistics = query.ToArray();
            }

            return View(dailyStatistics);
        }
    }
}

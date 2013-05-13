using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ncqrs.Eventing.ServiceModel.Bus;

using MyNotes.Events;
using MyNotes.ReadModel.Context;
using MyNotes.ReadModel.Types;

namespace MyNotes.Denormalizers
{
    public class NoteDenormalizer : IEventHandler<NoteAdded>,
                                    IEventHandler<NoteTextChanged>
    {
        public void Handle(IPublishedEvent<NoteAdded> evnt)
        {
            using (var context = new ReadModelContext())
            {
                var existing = context.Notes.SingleOrDefault(x => x.Id == evnt.Payload.Id);

                if (existing != null)
                    return;

                var newItem = new Note
                {
                    Id = evnt.Payload.Id,
                    Text = evnt.Payload.Text,
                    CreationDate = evnt.Payload.CreationDate
                };

                context.Notes.Add(newItem);
                context.SaveChanges();
            }
        }

        public void Handle(IPublishedEvent<NoteTextChanged> evnt)
        {
            using (var context = new ReadModelContext())
            {
                var itemToUpdate = context.Notes.Single(item => item.Id == evnt.EventSourceId);
                itemToUpdate.Text = evnt.Payload.Text;

                context.SaveChanges();
            }
        }
    }
}

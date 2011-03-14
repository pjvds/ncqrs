using System.Linq;
using Events;
using Ncqrs.Eventing.ServiceModel.Bus;
using ReadModel;

namespace ApplicationService.Denormalizers
{
    public class NoteItemDenormalizer : IEventHandler<NewNoteAdded>,
                                        IEventHandler<NoteTextChanged>
    {
        public void Handle(IPublishedEvent<NewNoteAdded> evnt)
        {
            using (var context = new ReadModelContainer())
            {
                var existing = context.NoteItemSet.SingleOrDefault(x => x.Id == evnt.Payload.NoteId);
                if (existing != null)
                {
                    return;                    
                }

                var newItem = new NoteItem
                {
                    Id = evnt.Payload.NoteId,
                    Text = evnt.Payload.Text,
                    CreationDate = evnt.Payload.CreationDate
                };

                context.NoteItemSet.AddObject(newItem);
                context.SaveChanges();
            }
        }

        public void Handle(IPublishedEvent<NoteTextChanged> evnt)
        {
            using (var context = new ReadModelContainer())
            {
                var itemToUpdate = context.NoteItemSet.Single(item => item.Id == evnt.EventSourceId);
                itemToUpdate.Text = evnt.Payload.NewText;

                context.SaveChanges();
            }
        }
    }
}
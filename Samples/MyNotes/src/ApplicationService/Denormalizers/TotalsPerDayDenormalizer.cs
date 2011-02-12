using System.Linq;
using Events;
using Ncqrs.Eventing.ServiceModel.Bus;
using ReadModel;

namespace ApplicationService.Denormalizers
{
    public class TotalsPerDayDenormalizer : IEventHandler<NewNoteAdded>,
                                            IEventHandler<NoteTextChanged>
    {
        public void Handle(IPublishedEvent<NewNoteAdded> evnt)
        {
            using (var context = new ReadModelContainer())
            {
                var date = evnt.Payload.CreationDate.Date;
                var totalsForDate = context.TotalsPerDayItemSet.SingleOrDefault(i => i.Date == date);

                if (totalsForDate == null)
                {
                    totalsForDate = new TotalsPerDayItem {Id = 1, Date = date};
                    context.TotalsPerDayItemSet.AddObject(totalsForDate);
                }

                totalsForDate.NewCount++;
                context.SaveChanges();
            }
        }

        public void Handle(IPublishedEvent<NoteTextChanged> evnt)
        {
            using (var context = new ReadModelContainer())
            {
                var date = evnt.EventTimeStamp.Date;
                var totalsForDate = context.TotalsPerDayItemSet.SingleOrDefault(i => i.Date == date);

                if (totalsForDate == null)
                {
                    totalsForDate = new TotalsPerDayItem {Date = date};
                    context.TotalsPerDayItemSet.AddObject(totalsForDate);
                }

                totalsForDate.EditCount++;

                context.SaveChanges();
            }
        }
    }
}
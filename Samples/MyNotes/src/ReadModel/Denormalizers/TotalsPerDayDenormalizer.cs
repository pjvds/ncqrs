﻿using System.Linq;
using Events;
using Ncqrs.Eventing.ServiceModel.Bus;

namespace ReadModel.Denormalizers
{
    public class TotalsPerDayDenormalizer : IEventHandler<NewNoteAdded>,
                                            IEventHandler<NoteTextChanged>
    {
        public void Handle(NewNoteAdded evnt)
        {
            using (var context = new ReadModelContainer())
            {
                var date = evnt.CreationDate.Date;
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

        public void Handle(NoteTextChanged evnt)
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
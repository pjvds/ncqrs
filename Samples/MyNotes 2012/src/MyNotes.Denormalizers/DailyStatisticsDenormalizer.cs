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
    public class DailyStatisticsDenormalizer : IEventHandler<NoteAdded>,
                                               IEventHandler<NoteTextChanged>
    {
        public void Handle(IPublishedEvent<NoteAdded> e)
        {
            using (var context = new ReadModelContext())
            {
                var date = e.Payload.CreationDate.Date;
                var totalsForDate = context.DailyStatistics.SingleOrDefault(i => i.Date == date);

                if (totalsForDate == null)
                {
                    totalsForDate = new DailyStatistics { Id = 0, Date = date };
                    context.DailyStatistics.Add(totalsForDate);
                }

                totalsForDate.NewCount++;
                context.SaveChanges();
            }
        }

        public void Handle(IPublishedEvent<NoteTextChanged> e)
        {
            using (var context = new ReadModelContext())
            {
                var date = e.EventTimeStamp.Date;
                var totalsForDate = context.DailyStatistics.SingleOrDefault(i => i.Date == date);

                if (totalsForDate == null)
                {
                    totalsForDate = new DailyStatistics { Date = date };
                    context.DailyStatistics.Add(totalsForDate);
                }

                totalsForDate.EditCount++;

                context.SaveChanges();
            }
        }
    }
}

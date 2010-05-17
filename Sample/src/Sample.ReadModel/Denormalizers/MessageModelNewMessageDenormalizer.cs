using System;
using Ncqrs.Eventing.Denormalization;
using Sample.Events;
using Sample.ReadModel.Properties;
using System.Linq;

namespace Sample.ReadModel.Denormalizers
{
    public class MessageModelNewMessageDenormalizer : Denormalizer<NewMessageAdded>
    {
        public override void DenormalizeEvent(NewMessageAdded evnt)
        {
            using(var context = new ReadModelDataContext(Settings.Default.ReadModelConnection))
            {
                var newModel = new MessageModel();
                newModel.Id = evnt.MessageId;
                newModel.Text = evnt.Text;
                newModel.CreationDate = evnt.CreationDate;

                context.MessageModels.InsertOnSubmit(newModel);
                context.SubmitChanges();
            }
        }
    }
}

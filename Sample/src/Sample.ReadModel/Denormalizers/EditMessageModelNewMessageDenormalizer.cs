using System;
using Ncqrs.Eventing.Denormalization;
using Sample.Events;

namespace Sample.ReadModel.Denormalizers
{
    public class EditMessageModelNewMessageDenormalizer : Denormalizer<NewMessageAdded>
    {
        public override void DenormalizeEvent(NewMessageAdded evnt)
        {
            //var newModel = new Sample.ReadModel.EditMessageModel
            //{
            //    Id = evnt.MessageId,
            //    Text = evnt.Text,
            //    CreationDate = evnt.CreationDate
            //}

            //var container = new ReadModelContainer();
            //container.EditMessageModelSet.Attach(newModel);
            //container.SaveChanges();
        }
    }
}

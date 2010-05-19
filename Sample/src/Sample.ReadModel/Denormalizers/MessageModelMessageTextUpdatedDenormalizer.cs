using System;
using Ncqrs.Eventing.Denormalization;
using Sample.Events;

namespace Sample.ReadModel.Denormalizers
{
    public class MessageModelMessageTextUpdatedDenormalizer : Denormalizer<MessageTextUpdated>
    {
        public override void DenormalizeEvent(MessageTextUpdated evnt)
        {
            //var container = new ReadModelContainer();
            //container.CreateQuery<Sample.ReadModel.MessageModel>().

            //var newModel = container.MessageModelSet..CreateObject();
            //newModel.Id = evnt.MessageId;
            //newModel.Text = evnt.Text;
            //newModel.CreationDate = evnt.CreationDate;

            //container.SaveChanges();

            //using (var repository = new ReadRepository<IMessageModel>())
            //{
            //    var spec = repository.New();
            //    spec.Id = evnt.MessageId;

            //    var modelToUpdate = repository.FindOne(spec.Document);
            //    modelToUpdate.Text = evnt.UpdatedMessageText;

            //    repository.Update(modelToUpdate);
            //}
        }
    }
}

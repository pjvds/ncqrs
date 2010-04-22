using System;
using Ncqrs.Eventing.Denormalization;
using Sample.Events;

namespace Sample.ReadModel.Denormalizers.MessageModel
{
    public class MessageModelMessageTextUpdatedDenormalizer : Denormalizer<MessageTextUpdated>
    {
        public override void DenormalizeEvent(MessageTextUpdated evnt)
        {
            using (var repository = new ReadRepository<IMessageModel>())
            {
                var spec = repository.New();
                spec.Id = evnt.MessageId;

                var modelToUpdate = repository.FindOne(spec.Document);
                modelToUpdate.Text = evnt.UpdatedMessageText;

                repository.Update(modelToUpdate);
            }
        }
    }
}

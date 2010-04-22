using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Domain.Denormalization;
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

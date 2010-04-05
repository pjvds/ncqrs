using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Eventing.Denormalization;
using Sample.Events;
using MongoDB.Driver;
using MongoDB.Emitter;

namespace Sample.ReadModel.Denormalizers
{
    public class MessageTextUpdatedDenormalizer : Denormalizer<MessageTextUpdated>
    {
        public override void DenormalizeEvent(MessageTextUpdated evnt)
        {
            DenormalizeForMessageModel(evnt);
            DenormalizeForEditMessageModel(evnt);
        }

        private void DenormalizeForEditMessageModel(MessageTextUpdated evnt)
        {
            using (var repository = new ReadRepository<IEditMessageModel>())
            {
                var spec = repository.New();
                spec.Id = evnt.MessageId;

                var modelToUpdate = repository.FindOne(spec);

                var previousText = WrapperFactory.Instance.New<IPreviousTextModel>();
                previousText.ChangeDate = evnt.ChangeDate;
                previousText.Text = modelToUpdate.Text;

                var previousTexts = new List<IPreviousTextModel>(modelToUpdate.TextChanges);
                previousTexts.Add(previousText);

                modelToUpdate.TextChanges = WrapperFactory.Instance.NewArrayWrapper(previousTexts);

                repository.Update(modelToUpdate);
            }
        }

        private void DenormalizeForMessageModel(MessageTextUpdated evnt)
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

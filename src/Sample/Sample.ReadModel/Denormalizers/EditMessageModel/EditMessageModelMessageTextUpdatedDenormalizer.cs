using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Eventing.Denormalization;
using Sample.Events;
using MongoDB.Emitter;

namespace Sample.ReadModel.Denormalizers.EditMessageModel
{
    public class EditMessageModelMessageTextUpdatedDenormalizer : Denormalizer<MessageTextUpdated>
    {
        public override void DenormalizeEvent(MessageTextUpdated evnt)
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

                modelToUpdate.Text = evnt.UpdatedMessageText;

                repository.Update(modelToUpdate);
            }
        }
    }
}

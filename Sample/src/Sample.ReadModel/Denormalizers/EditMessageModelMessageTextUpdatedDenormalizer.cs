using System;
using System.Collections.Generic;
using Ncqrs.Eventing.Denormalization;
using Sample.Events;

namespace Sample.ReadModel.Denormalizers
{
    public class EditMessageModelMessageTextUpdatedDenormalizer : Denormalizer<MessageTextUpdated>
    {
        public override void DenormalizeEvent(MessageTextUpdated evnt)
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

            //using (var repository = new ReadRepository<IEditMessageModel>())
            //{
            //    var spec = repository.New();
            //    spec.Id = evnt.MessageId;

            //    var modelToUpdate = repository.FindOne(spec);

            //    var previousText = WrapperFactory.Instance.New<IPreviousTextModel>();
            //    previousText.ChangeDate = evnt.ChangeDate;
            //    previousText.Text = modelToUpdate.Text;

            //    var previousTexts = new List<IPreviousTextModel>(modelToUpdate.TextChanges);
            //    previousTexts.Add(previousText);

            //    modelToUpdate.TextChanges = WrapperFactory.Instance.NewArrayWrapper(previousTexts);

            //    modelToUpdate.Text = evnt.UpdatedMessageText;

            //    repository.Update(modelToUpdate);
            //}
        }
    }
}

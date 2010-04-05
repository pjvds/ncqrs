using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Eventing.Denormalization;
using Sample.Events;
using MongoDB.Emitter;

namespace Sample.ReadModel.Denormalizers.EditMessageModel
{
    public class EditMessageModelNewMessageDenormalizer : Denormalizer<NewMessageAdded>
    {
        public override void DenormalizeEvent(NewMessageAdded evnt)
        {
            using(var repository = new ReadRepository<IEditMessageModel>())
            {
                var newModel = repository.New();
                newModel.Id = evnt.MessageId;
                newModel.Text = evnt.Text;
                newModel.CreationDate = evnt.CreationDate;
                newModel.TextChanges = WrapperFactory.Instance.NewArrayWrapper<IPreviousTextModel>();

                repository.Insert(newModel);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Eventing.Denormalization;
using Sample.Events;
using MongoDB.Emitter;

namespace Sample.ReadModel.Denormalizers.MessageModel
{
    public class MessageModelNewMessageDenormalizer : Denormalizer<NewMessageAdded>
    {
        public override void DenormalizeEvent(NewMessageAdded evnt)
        {
            using (var repository = new ReadRepository<IMessageModel>())
            {
                var newModel = repository.New();

                newModel.Id = evnt.MessageId;
                newModel.Text = evnt.Text;
                newModel.CreationDate = evnt.CreationDate;

                repository.Insert(newModel);
            }
        }
    }
}

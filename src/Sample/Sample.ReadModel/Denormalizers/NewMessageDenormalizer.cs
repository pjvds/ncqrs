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
    public class NewMessageDenormalizer : Denormalizer<NewMessageAdded>
    {
        public override void DenormalizeEvent(NewMessageAdded evnt)
        {
            using (Mongo mongo = new Mongo())
            {
                mongo.Connect();
                var db = mongo.GetDatabase("ReadModel");
                var messageModelCol = db.GetCollection("MessageModel");

                var newModel = WrapperFactory.Instance.New<IMessageModel>();

                newModel.Id = evnt.MessageId;
                newModel.Text = evnt.Text;
                newModel.CreationDate = evnt.CreationDate;

                messageModelCol.Insert(newModel.Document);

                var editMessageModelCol = db.GetCollection("EditMessageModel");

                var newEditMessageModel = WrapperFactory.Instance.New<IEditMessageModel>();
                newEditMessageModel.Id = evnt.MessageId;
                newEditMessageModel.Text = evnt.Text;
                newEditMessageModel.CreationDate = evnt.CreationDate;
                newEditMessageModel.TextChanges = WrapperFactory.Instance.NewArrayWrapper<IPreviousTextModel>();

                editMessageModelCol.Insert(newEditMessageModel.Document);
            }
        }
    }
}

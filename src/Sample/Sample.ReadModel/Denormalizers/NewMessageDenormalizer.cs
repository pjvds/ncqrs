using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Eventing.Denormalization;
using Sample.Events;
using MongoDB.Driver;

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

                var newMessageModel = new MessageModel
                {
                    Id = evnt.MessageId,
                    Text = evnt.Text,
                    CreationDate = evnt.CreationDate
                };

                messageModelCol.Insert(newMessageModel.InnerDocument);

                var editMessageModelCol = db.GetCollection("EditMessageModel");

                var newEditMessageModel = new EditMessageModel
                {
                    Id = evnt.MessageId,
                    Text = evnt.Text,
                    CreationDate = evnt.CreationDate,
                    TextChanges = new PreviousTextModel[0]
                };

                editMessageModelCol.Insert(newEditMessageModel.InnerDocument);
            }
        }
    }
}

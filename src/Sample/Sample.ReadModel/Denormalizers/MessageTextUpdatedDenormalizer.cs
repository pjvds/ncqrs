using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Eventing.Denormalization;
using Sample.Events;
using MongoDB.Driver;

namespace Sample.ReadModel.Denormalizers
{
    public class MessageTextUpdatedDenormalizer : Denormalizer<MessageTextUpdated>
    {
        public override void DenormalizeEvent(MessageTextUpdated evnt)
        {
            using (Mongo mongo = new Mongo())
            {
                mongo.Connect();
                var db = mongo.GetDatabase("ReadModel");
                var collection = db.GetCollection("MessageModel");

                var spec = new MessageModel();
                spec.Id = evnt.MessageId;

                var document = collection.Find(spec.InnerDocument).Documents.First();

                var messageModelToUpdate = new MessageModel();
                messageModelToUpdate.InnerDocument = document;

                messageModelToUpdate.Text = evnt.UpdatedMessageText;

                collection.Update(messageModelToUpdate.InnerDocument);
            }
        }
    }
}

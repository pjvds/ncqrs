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
                var messageModelCol = db.GetCollection("MessageModel");

                var spec = new MessageModel();
                spec.Id = evnt.MessageId;

                var document = messageModelCol.Find(spec.InnerDocument).Documents.First();

                var messageModelToUpdate = new MessageModel();
                messageModelToUpdate.InnerDocument = document;

                messageModelToUpdate.Text = evnt.UpdatedMessageText;

                messageModelCol.Update(messageModelToUpdate.InnerDocument);


                var editMessageModelCol = db.GetCollection("EditMessageModel");

                var spec2 = new EditMessageModel();
                spec2.Id = evnt.MessageId;
                var document2 = editMessageModelCol.Find(spec2.InnerDocument).Documents.First();

                var editMessageModelToUpdate = new EditMessageModel() { InnerDocument = document2 };
                var newMessageTextUpdate = new PreviousTextModel() { ChangeDate = evnt.ChangeDate, Text = editMessageModelToUpdate.Text };
                editMessageModelToUpdate.Text = evnt.UpdatedMessageText;
                var newChanges = new List<PreviousTextModel>(editMessageModelToUpdate.TextChanges);
                newChanges.Add(newMessageTextUpdate);
                editMessageModelToUpdate.TextChanges = newChanges.ToArray();

                editMessageModelCol.Update(editMessageModelToUpdate.InnerDocument);
            }
        }
    }
}

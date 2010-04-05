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
            using (Mongo mongo = new Mongo())
            {
                mongo.Connect();

                DenormalizeForMessageModel(mongo, evnt);
                DenormalizeForEditMessageModel(mongo, evnt);

                var db = mongo.GetDatabase("ReadModel");

            }
        }

        private void DenormalizeForEditMessageModel(Mongo mongo, MessageTextUpdated evnt)
        {
            var db = mongo.GetDatabase("ReadModel");
            var collection = db.GetCollection("EditMessageModel");

            var spec = WrapperFactory.Instance.New<IEditMessageModel>();
            spec.Id = evnt.MessageId;
            var document = collection.Find(spec.Document).Documents.First();

            var editMessageModelToUpdate = WrapperFactory.Instance.New<IEditMessageModel>(document);
            var newMessageTextUpdate = WrapperFactory.Instance.New<IPreviousTextModel>();
            newMessageTextUpdate.ChangeDate = evnt.ChangeDate;
            newMessageTextUpdate.Text = editMessageModelToUpdate.Text;

            editMessageModelToUpdate.Text = evnt.UpdatedMessageText;
            var newChanges = new List<IPreviousTextModel>(editMessageModelToUpdate.TextChanges);
            newChanges.Add(newMessageTextUpdate);
            editMessageModelToUpdate.TextChanges = WrapperFactory.Instance.NewArrayWrapper<IPreviousTextModel>(newChanges);

            collection.Update(editMessageModelToUpdate.Document);
        }

        private void DenormalizeForMessageModel(Mongo mongo, MessageTextUpdated evnt)
        {
            var db = mongo.GetDatabase("ReadModel");
            var collection = db.GetCollection("MessageModel");

            var spec = WrapperFactory.Instance.New<IMessageModel>();
            spec.Id = evnt.MessageId;


            var document = collection.FindOne(spec.Document);
            var found = WrapperFactory.Instance.New<IMessageModel>(document);
            found.Text = evnt.UpdatedMessageText;
            
            collection.Update(found.Document);
        }
    }
}

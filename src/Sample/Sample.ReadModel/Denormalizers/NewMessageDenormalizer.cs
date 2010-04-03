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
                var collection = db.GetCollection("MessageModel");

                var newMessageModel = new MessageModel
                {
                    Id = evnt.MessageId,
                    Text = evnt.Text,
                    CreationDate = evnt.CreationDate
                };

                collection.Insert(newMessageModel.InnerDocument);
            }
        }
    }
}

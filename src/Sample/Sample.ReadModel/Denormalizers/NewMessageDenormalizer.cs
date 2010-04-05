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




            }
        }
    }
}

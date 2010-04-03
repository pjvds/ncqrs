using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;

namespace Sample.ReadModel
{
    public class ReadRepository<TModel> where TModel : MongoReadModelEntity, new()
    {
        private Mongo _mongo;

        public ReadRepository ()
	    {
            _mongo = new Mongo();
        }

        protected void Connect()
        {
            _mongo.Connect();
        }

        protected void Disconnect()
        {
            _mongo.Disconnect();
        }

        protected IMongoCollection Collection
        {
            get
            {
                var db = _mongo.GetDatabase("ReadModel");
                return db.GetCollection(typeof(TModel).Name);
            }
        }

        public IEnumerable<TModel> FindAll()
        {
            Connect();

            try
            {
                var cursor = Collection.FindAll();
                var documents = cursor.Documents;

                foreach(var doc in documents)
                {
                    var model = new TModel();
                    model.InnerDocument = doc;
                    yield return model;
                }
            }
            finally
            {
                Disconnect();
            }
        }
    }
}
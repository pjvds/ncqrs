using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using MongoDB.Emitter;

namespace Sample.ReadModel
{
    public class ReadRepository<TModel> : IDisposable where TModel : class, IDocumentWrapper
    {
        private Mongo _mongo;

        public ReadRepository()
        {
            _mongo = new Mongo();
            _mongo.Connect();

            var db = _mongo.GetDatabase("ReadModel");
            Collection = db.GetCollection(typeof(TModel).Name);
        }

        public void Dispose()
        {
            _mongo.Dispose();
        }

        public TModel New()
        {
            return WrapperFactory.Instance.New<TModel>();
        }

        protected IMongoCollection Collection
        {
            get;
            private set;
        }

        public IEnumerable<TModel> FindAll()
        {
            return FindAll(null);
        }

        public void Insert(TModel model)
        {
            Collection.Insert(model.Document, true);
        }

        public void Update(TModel model)
        {
            Collection.Update(model.Document, true);
        }

        public IEnumerable<TModel> FindAll(Document sort)
        {
            var cursor = Collection.FindAll();

            if (sort != null)
                cursor.Sort(sort);

            var documents = cursor.Documents;

            foreach (var doc in documents)
            {
                yield return WrapperFactory.Instance.New<TModel>(doc);
            }
        }

        public TModel FindOne(TModel spec)
        {
            return FindOne(spec);
        }

        public TModel FindOne(Document spec)
        {
            TModel model = null;
            var document = Collection.FindOne(spec);

            if (document != null)
            {
                model = WrapperFactory.Instance.New<TModel>(document);
            }

            return model;
        }

        public IEnumerable<TModel> Find(TModel sample)
        {
            return Find(sample.Document, null);
        }

        public IEnumerable<TModel> Find(Document document, Document sort)
        {
            var cursor = Collection.Find(document);

            if (sort != null)
                cursor.Sort(sort);

            var documents = cursor.Documents;

            foreach (var doc in documents)
            {
                yield return WrapperFactory.Instance.New<TModel>(doc);
            }
        }
    }
}
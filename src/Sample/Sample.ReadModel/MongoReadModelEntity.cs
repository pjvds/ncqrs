using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;

namespace Sample.ReadModel
{
    public abstract class MongoReadModelEntity
    {
        protected internal Document InnerDocument
        {
            get;
            internal set;
        }

        public MongoReadModelEntity()
        {
            InnerDocument = new Document();
        }
    }
}

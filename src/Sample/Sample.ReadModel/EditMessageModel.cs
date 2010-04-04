using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using System.Collections;

namespace Sample.ReadModel
{
    public class PreviousTextModel : MongoReadModelEntity
    {
        public DateTime ChangeDate
        {
            get
            {
                return (DateTime)InnerDocument["ChangeDate"];
            }
            internal set
            {
                InnerDocument["ChangeDate"] = value;
            }
        }

        public String Text
        {
            get
            {
                return (String)InnerDocument["Text"];
            }
            internal set
            {
                InnerDocument["Text"] = value;
            }
        }
    }

    public class EditMessageModel : MongoReadModelEntity
    {
        public Guid Id
        {
            get
            {
                return (Guid)InnerDocument["Id"];
            }
            internal set
            {
                InnerDocument["Id"] = value;
            }
        }

        public String Text
        {
            get
            {
                return (String)InnerDocument["Text"];
            }
            internal set
            {
                InnerDocument["Text"] = value;
            }
        }

        public PreviousTextModel[] TextChanges
        {
            get
            {
                if (InnerDocument["TextChanges"] != null && ((IList)InnerDocument["TextChanges"]).Count > 0)
                {
                    var docs = (IList<Document>)InnerDocument["TextChanges"];
                    var models = new PreviousTextModel[docs.Count];

                    for (int i = 0; i < docs.Count; i++)
                    {
                        models[i] = new PreviousTextModel() { InnerDocument = docs[i] };
                    }

                    return models;
                }
                else
                {
                    return new PreviousTextModel[0];
                }
            }
            internal set
            {
                var docs = new Document[value.Length];

                for (int i = 0; i < value.Length; i++)
                {
                    docs[i] = value[i].InnerDocument;
                }

                InnerDocument["TextChanges"] = docs;
            }
        }

        public DateTime CreationDate
        {
            get
            {
                return (DateTime)InnerDocument["CreationDate"];
            }
            internal set
            {
                InnerDocument["CreationDate"] = value;
            }
        }
    }
}

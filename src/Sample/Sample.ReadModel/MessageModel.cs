using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;

namespace Sample.ReadModel
{
    public class MessageModel : MongoReadModelEntity
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
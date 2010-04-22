using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using System.Collections;
using MongoDB.Emitter;

namespace Sample.ReadModel
{
    public interface IEditMessageModel : IDocumentWrapper
    {
        Guid Id
        {
            get;
            set;
        }

        String Text
        {
            get;
            set;
        }

        ArrayWrapper<IPreviousTextModel> TextChanges
        {
            get;
            set;
        }

        DateTime CreationDate
        {
            get;
            set;
        }
    }
}

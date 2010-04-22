using System;
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

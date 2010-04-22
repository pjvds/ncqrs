using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Emitter;

namespace Sample.ReadModel
{
    public interface IPreviousTextModel : IDocumentWrapper
    {
        DateTime ChangeDate
        {
            get;
            set;
        }

        String Text
        {
            get;
            set;
        }
    }
}

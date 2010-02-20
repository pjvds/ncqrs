using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ncqrs.Eventing
{
    public interface IUniqueIdentifierGenerator
    {
        Guid GenerateNewId(EventSource eventSource);
    }
}

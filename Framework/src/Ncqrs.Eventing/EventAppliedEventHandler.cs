using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ncqrs.Domain
{
    public delegate void EventAppliedEventHandler(EventSource sender, EventAppliedEventArgs e);
}

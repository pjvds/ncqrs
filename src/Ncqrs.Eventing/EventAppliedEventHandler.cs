using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ncqrs.Eventing
{
    public delegate void EventAppliedEventHandler(EventSource sender, EventAppliedEventArgs e);
}

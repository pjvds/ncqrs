using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Eventing.Bus;

namespace Ncqrs.Config
{
    public interface IEnvironmentConfiguration
    {
        T Get<T>();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ncqrs.Config
{
    public interface IEnvironmentConfiguration
    {
        T Get<T>();

        Boolean TryGet<T>(out T result);
    }
}
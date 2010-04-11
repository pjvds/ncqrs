using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Domain;

namespace Ncqrs.Domain
{
    public interface IDomainEnvironment
    {
        IUnitOfWorkFactory CreateUnitOfWorkFactory();
    }
}

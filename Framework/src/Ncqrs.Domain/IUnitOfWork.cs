using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Domain.Storage;

namespace Ncqrs.Domain
{
    public interface IUnitOfWork : IDisposable
    {
        IDomainRepository Repository
        {
            get;
        }

        void Accept();
    }
}

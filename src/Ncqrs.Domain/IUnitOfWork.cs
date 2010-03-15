using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ncqrs.Domain
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Accepts the unit of work and persist the changes.
        /// </summary>
        void Accept();
    }
}

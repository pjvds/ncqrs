using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using Ncqrs.Commands;
using Ncqrs.Domain.Storage;

namespace Ncqrs.CommandHandling
{
    public abstract class CommandHandler<T> : ICommandHandler where T : ICommand
    {
        protected IDomainRepository Repository
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandler&lt;T&gt;"/> class.
        /// </summary>
        protected CommandHandler(IDomainRepository domainRepository)
        {
            Contract.Requires(domainRepository != null);

            Repository = domainRepository;
        }

        public abstract void Handle(T message);

        void ICommandHandler.Handle(ICommand command)
        {
            this.Handle((T)command);
        }
    }
}
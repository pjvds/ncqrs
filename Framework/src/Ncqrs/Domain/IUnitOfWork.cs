using System;
using Ncqrs.Domain.Storage;
using Ncqrs.Eventing;

namespace Ncqrs.Domain
{
    /// <summary>
    /// A context from within domain object can be changed.
    /// <example>
    /// <code>
    /// var factory = NcqrsEnvironment.Get{IUnitOfWorkFactory}();
    /// using (var work = factory.CreateUnitOfWork())
    /// {
    ///     // Create the new customer.
    ///     Customer newCustomer = new Customer();
    ///     newCustomer.Name = "Pieter Joost van de Sande";
    ///     
    ///     // Accept the work that has been done in the context.
    ///     work.Accept();
    /// }
    /// </code>
    /// </example>
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        IDomainRepository Repository
        {
            get;
        }

        void Accept();

        // TODO: Remove this from the public interface.
        void RegisterDirtyInstance(AggregateRoot dirtyAggregateRoot);
    }
}

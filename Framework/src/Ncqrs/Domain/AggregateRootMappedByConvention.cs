using System;
using Ncqrs.Eventing.Sourcing.Mapping;

namespace Ncqrs.Domain
{
    /// <summary>
    /// A aggregate root that uses convention to map internal event handlers.
    /// The following method should be mapped 
    /// <c>private void OnMyEvent(MyEvent e)</c>.
    /// </summary>
    /// <remarks><para>This aggregate root uses the 
    /// <see cref="ConventionBasedDomainSourcedEventHandlerMappingStrategy"/> to get
    /// the internal event handlers.</remarks>
    /// <example>
    /// An example of a aggregate root that inhered the 
    /// <see cref="AggregateRootMappedByConvention"/> class. The 
    /// <c>OnNewCustomerCreated</c> method is mapped due convention.
    /// <code lang="c#">
    /// public class Customer : AggregateRootMappedByConvention
    /// {
    ///     private string _name;
    /// 
    ///     public Customer(string name)
    ///     {
    ///         var e = new NewCustomerCreated(Id, name);
    ///         ApplyEvent(e);
    ///     }
    /// 
    ///     private void OnNewCustomerCreated(NewCustomerCreated e)
    ///     {
    ///         _name = e.Name;
    ///     }
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="ConventionBasedDomainSourcedEventHandlerMappingStrategy"/>
    public abstract class AggregateRootMappedByConvention : MappedAggregateRoot
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRootMappedByConvention"/> class.
        /// </summary>
        protected AggregateRootMappedByConvention()
            : base(new ConventionBasedEventHandlerMappingStrategy())
        {
        }

        protected AggregateRootMappedByConvention(Guid id)
            : base(id, new ConventionBasedEventHandlerMappingStrategy())
        {
        }
    }
}

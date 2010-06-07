//using System;
//using Ncqrs.Domain.Mapping;

//namespace Ncqrs.Domain
//{
//    /// <summary>
//    /// A aggregate root that uses convention to map internal event handlers.
//    /// The following method should be mapped 
//    /// <c>private void OnMyEvent(MyEvent e)</c>.
//    /// </summary>
//    /// <remarks><para>This aggregate root uses the 
//    /// <see cref="ConventionBasedDomainEventHandlerMappingStrategy"/> to get
//    /// the internal event handlers.</remarks>
//    /// <example>
//    /// An example of a aggregate root that inhered the 
//    /// <see cref="AggregateRootMappedByConvention"/> class. The 
//    /// <c>OnNewCustomerCreated</c> method is mapped due convention.
//    /// <code lang="c#">
//    /// public class Customer : AggregateRootMappedByConvention
//    /// {
//    ///     private string _name;
//    /// 
//    ///     public Customer(string name)
//    ///     {
//    ///         var e = new NewCustomerCreated(Id, name);
//    ///         ApplyEvent(e);
//    ///     }
//    /// 
//    ///     private void OnNewCustomerCreated(NewCustomerCreated e)
//    ///     {
//    ///         _name = e.Name;
//    ///     }
//    /// }
//    /// </code>
//    /// </example>
//    /// <seealso cref="ConventionBasedDomainEventHandlerMappingStrategy"/>
//    public class AggregateRootMappedByConvention : MappedAggregateRoot<AggregateRootMappedByConvention>
//    {        
//        public override void Initialize(object aggregateRootInstance)
//        {
//            MappingStrategy = new ConventionBasedDomainEventHandlerMappingStrategy();
//            base.Initialize(aggregateRootInstance);
//        }
//    }
//}

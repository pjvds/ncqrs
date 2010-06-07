//using System;
//using System.Collections.Generic;
//using Ncqrs.Domain.Mapping;

//namespace Ncqrs.Domain
//{
//    /// <summary>
//    /// A aggregate root that uses lambda style mapping to map internal event handlers. The following method should be mapped 
//    /// </summary>
//    /// <remarks>
//    /// This aggregate root uses the  <see cref="ExpressionBasedDomainEventHandlerMappingStrategy"/> to get the internal event handlers.
//    /// </remarks>
//    /// <seealso cref="ExpressionBasedDomainEventHandlerMappingStrategy"/>
//    public abstract class AggregateRootMappedWithExpressions : MappedAggregateRoot<AggregateRootMappedWithExpressions>
//    {
//        [NonSerialized]
//        private readonly IList<ExpressionHandler> _mappinghandlers = new List<ExpressionHandler>();

//        public override void Initialize(object aggregateRootInstance)
//        {
//            InitializeEventHandlers();
//            MappingStrategy = new ExpressionBasedDomainEventHandlerMappingStrategy(_mappinghandlers);
//            base.Initialize(aggregateRootInstance);
//        }
        
//        /// <summary>
//        /// Maps the given generic eventtype to the expressed handler.
//        /// </summary>
//        /// <typeparam name="T">This should always be a <see cref="DomainEvent"/>.</typeparam>
//        /// <returns>An <see cref="ExpressionHandler{T}"/>which allows us to define the mapping to a handler.</returns>
//        protected ExpressionHandler<T> Map<T>() where T : DomainEvent
//        {
//            var handler = new ExpressionHandler<T>();
//            _mappinghandlers.Add(handler);

//            return handler;
//        }

//        ///<summary>
//        /// Defines the method that derived types need to implement to support strongly typed mapping.
//        ///</summary>
//        public abstract void InitializeEventHandlers();
//    }
//}
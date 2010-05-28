using System.Collections.Generic;
using Ncqrs.Domain.Mapping;

namespace Ncqrs.Domain
{
    ///<summary>
    ///</summary>
    public abstract class AggregateRootMappedWithExpressions : MappedAggregateRoot<AggregateRootMappedWithExpressions>
    {
        private readonly IList<ExpressionHandler> _mappinghandlers = new List<ExpressionHandler>();

        internal IList<ExpressionHandler> MappingHandlers
        {
            get { return _mappinghandlers; }
        }

        protected AggregateRootMappedWithExpressions() 
            : base(new ExpressionBasedDomainEventHandlerMappingStrategy())
        {
            /* I know, calling virtual methods from the constructor isn't the smartest thing to do
             * but in this case it doesn't really matter because the implemented 
             * method isn't (and shouldn't be) using any derived resources
            **/
            InitializeEventHandlers();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected ExpressionHandler<T> Map<T>() where T : DomainEvent
        {
            var handler = new ExpressionHandler<T>();
            _mappinghandlers.Add(handler);

            return handler;
        }

        ///<summary>
        ///</summary>
        public abstract void InitializeEventHandlers();
    }
}
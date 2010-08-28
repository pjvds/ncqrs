using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Domain;
using Ncqrs.Eventing.Sourcing;
using NUnit.Framework;

namespace Ncqrs.Tests.Domain
{
    [TestFixture]
    public class EntityTests
    {
        public class Customer : AggregateRootMappedByConvention
        {
            private readonly List<Order> _orders = new List<Order>();

            public void CreateOrder(int id)
            {
                ApplyEvent(new OrderCreatedEvent(id));
            }

            public void CreateOrderLine(int orderId, decimal value)
            {
                _orders.First(x => x.Id == orderId).CreateLine(value);
            }

            public void OnOrderCreated(OrderCreatedEvent evnt)
            {
                _orders.Add(new Order(this, evnt.Id));
            }
        }

        public class Order : Entity
        {
            private readonly int _id;
            private readonly List<OrderLine> _lines = new List<OrderLine>();

            public Order(AggregateRoot parent, int id)
                : base(parent)
            {
                RegisterHandler(new TypeAndCallbackThresholdedActionBasedDomainEventHandler(
                                    OnOrderLineCreated, x => x is OrderLineCreatedEvent &&((OrderLineCreatedEvent)x).OrderId == _id, typeof(OrderLineCreatedEvent)));
                _id = id;
            }

            public int Id
            {
                get { return _id; }
            }

            public void CreateLine(decimal value)
            {
                ApplyEvent(new OrderLineCreatedEvent(_id, value));
            }

            public void OnOrderLineCreated(SourcedEvent evnt)
            {
                _lines.Add(new OrderLine(((OrderLineCreatedEvent)evnt).Value));
            }
        }

        public class OrderCreatedEvent : SourcedEvent
        {
            private readonly int _id;

            public OrderCreatedEvent(int id)
            {
                _id = id;
            }

            public int Id
            {
                get { return _id; }
            }
        }

        public class OrderLineCreatedEvent : SourcedEvent
        {
            private readonly int _orderId;
            private readonly decimal _value;

            public OrderLineCreatedEvent(int orderId, decimal value)
            {
                _orderId = orderId;
                _value = value;
            }

            public decimal Value
            {
                get { return _value; }
            }

            public int OrderId
            {
                get { return _orderId; }
            }
        }

        public class OrderLine
        {
            private readonly decimal _value;

            public OrderLine(decimal value)
            {
                _value = value;
            }

            public decimal Value
            {
                get { return _value; }
            }
        }

        [Test]
        public void Creating_an_entity_should_generate_event()
        {
            using (NcqrsEnvironment.Get<IUnitOfWorkFactory>().CreateUnitOfWork())
            {
                var theAggregate = new Customer();
                theAggregate.CreateOrder(1);
                theAggregate.CreateOrderLine(1, 10);
                theAggregate.CreateOrderLine(1, 20);
                theAggregate.CreateOrder(2);
                theAggregate.CreateOrderLine(2, 30);
            }
        }

    }
   
}

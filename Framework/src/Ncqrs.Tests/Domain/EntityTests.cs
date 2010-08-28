using System;
using System.Collections.Generic;
using System.Linq;
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

            public void CreateOrder(Guid id)
            {
                ApplyEvent(new OrderCreatedEvent(id));
            }

            public void CreateOrderLine(Guid orderId, decimal value)
            {
                _orders.First(x => x.EntityId == orderId).CreateLine(value);
            }

            public void OnOrderCreated(OrderCreatedEvent evnt)
            {
                _orders.Add(new Order(this, evnt.OrderId));
            }
        }

        public class Order : EntityMappedByConvention
        {
            private readonly List<OrderLine> _lines = new List<OrderLine>();

            public Order(AggregateRoot parent, Guid entityId)
                : base(parent, entityId)
            {
            }

            public void CreateLine(decimal value)
            {
                ApplyEvent(new OrderLineCreatedEvent(value));
            }

            public void OnOrderLineCreated(SourcedEvent evnt)
            {
                _lines.Add(new OrderLine(((OrderLineCreatedEvent)evnt).Value));
            }
        }

        public class OrderCreatedEvent : SourcedEvent
        {
            private readonly Guid _orderId;

            public OrderCreatedEvent(Guid orderId)
            {
                _orderId = orderId;
            }

            public Guid OrderId
            {
                get { return _orderId; }
            }
        }

        public class OrderLineCreatedEvent : SourcedEntityEvent
        {
            private readonly decimal _value;

            public OrderLineCreatedEvent(decimal value)
            {
                _value = value;
            }

            public decimal Value
            {
                get { return _value; }
            }

            public Guid OrderId { get { return EntityId; }}
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
            var orderId1 = Guid.NewGuid();
            var orderId2 = Guid.NewGuid();

            var theAggregate = new Customer();
            theAggregate.CreateOrder(orderId1);
            theAggregate.CreateOrderLine(orderId1, 10);
            theAggregate.CreateOrderLine(orderId1, 20);
            theAggregate.CreateOrder(orderId2);
            theAggregate.CreateOrderLine(orderId2, 30);
        }
    }
}

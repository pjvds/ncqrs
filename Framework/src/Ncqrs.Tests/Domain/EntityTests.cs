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
            private bool _sent;
            private Address _sentToAddress;
            private readonly List<OrderLine> _lines = new List<OrderLine>();

            public Order(AggregateRoot parent, int id)
                : base(parent)
            {
                RegisterHandler(new TypeAndCallbackThresholdedActionBasedDomainEventHandler(
                                    OnOrderLineCreated, x => x is OrderLineCreatedEvent &&((OrderLineCreatedEvent)x).OrderId == _id, typeof(OrderLineCreatedEvent)));

                RegisterHandler(new TypeAndCallbackThresholdedActionBasedDomainEventHandler<OrderSentToAddress>(
                                    OnOrderSentToAddress, (x) => x.OrderId == Id));
                _id = id;
            }

            public int Id
            {
                get { return _id; }
            }

            public void Send(Address address)
            {
                ApplyEvent(new OrderSentToAddress(Id, address.Street, address.PostalCode, address.City,
                    address.State, address.County));
            }

            public void CreateLine(decimal value)
            {
                ApplyEvent(new OrderLineCreatedEvent(_id, value));
            }

            protected void OnOrderLineCreated(SourcedEvent evnt)
            {
                _lines.Add(new OrderLine(((OrderLineCreatedEvent)evnt).Value));
            }

            protected void OnOrderSentToAddress(OrderSentToAddress e)
            {
                _sent = true;
                _sentToAddress = new Address(e.Street, e.PostalCode, e.City, e.State, e.County);
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

        public class OrderSentToAddress : SourcedEvent
        {
            public int OrderId { get; set; }
            public string Street { get; set; }
            public string PostalCode { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string County { get; set; }

            public OrderSentToAddress(int orderId, string street, string postalCode, string city, string state, string county)
            {
                OrderId = orderId;
                Street = street;
                PostalCode = postalCode;
                City = city;
                State = state;
                County = county;
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

        public class Address
        {
            public string Street { get; private set; }
            public string PostalCode { get; private set; }
            public string City { get; private set; }
            public string State { get; private set; }
            public string County { get; private set; }

            public Address(string street, string postalCode, string city, string state, string county)
            {
                Street = street;
                PostalCode = postalCode;
                City = city;
                State = state;
                County = county;
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

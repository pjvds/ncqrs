using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Ncqrs.Spec
{
    [Specification]
    public abstract class EventSerializationFixture<T>
        : BaseTestFixture
    {

        protected T OriginalEvent { get; private set; }
        protected string SerializedEvent { get; private set; }
        protected T DeserializedEvent { get; private set; }

        protected abstract T GivenEvent();
        protected abstract string Serialize(T @event);
        protected abstract T Deserialize(string serializedEventData);

        protected override void Given()
        {
            OriginalEvent = GivenEvent();
            base.Given();
        }

        protected override void When()
        {
            SerializedEvent = Serialize(OriginalEvent);
            DeserializedEvent = Deserialize(SerializedEvent);
        }

        [Then]
        public void it_should_not_throw()
        {
            Assert.That(CaughtException, Is.Null);
        }

        [Then]
        public void the_serialized_event_should_not_be_null()
        {
            Assert.That(SerializedEvent, Is.Not.Null);
        }

        [Then]
        public void the_serialized_event_should_not_be_empty()
        {
            Assert.That(SerializedEvent, Is.Not.Empty);
        }

        [Then]
        public void the_deserialized_event_should_not_be_null()
        {
            Assert.That(DeserializedEvent, Is.Not.Null);
        }

        [Then]
        public void the_value_of_each_public_property_is_the_same()
        {
            var inconclusiveItems = new List<string>();

            var props = GetProperties();
            var indexedProps = props.Where(p => p.GetIndexParameters().Any());

            foreach (var prop in indexedProps)
                inconclusiveItems.Add(
                    string.Format(
                        "{0} is an indexed property and can't be tested automatically",
                        prop.Name));

            var unindexedProps = props.Except(indexedProps);

            TestItems(
                unindexedProps,
                (p, obj) => ((PropertyInfo)p).GetValue(obj, new object[0]),
                p => ((PropertyInfo)p).PropertyType,
                inconclusiveItems);

            if (inconclusiveItems.Any())
                Assert.Inconclusive(string.Join(Environment.NewLine,
                                                inconclusiveItems));

        }

        [Then]
        public void the_value_of_each_public_field_is_the_same()
        {
            var inconclusiveItems = new List<string>();

            TestItems(
                GetFields(),
                (f, obj) => ((FieldInfo)f).GetValue(obj),
                f => ((FieldInfo)f).FieldType,
                inconclusiveItems);

            if (inconclusiveItems.Any())
                Assert.Inconclusive(string.Join(Environment.NewLine,
                                                inconclusiveItems));

        }

        protected virtual void TestItems(IEnumerable<MemberInfo> publicMembers,
            Func<MemberInfo, object, object> getValue,
            Func<MemberInfo, Type> getMemberType,
            IList<string> inconclusiveItems)
        {
            var originalValues = publicMembers
                .Select(p => new
                {
                    p.Name,
                    Type = getMemberType(p),
                    value = getValue(p, OriginalEvent)
                });

            var resultValues = publicMembers
                .Select(p => getValue(p, DeserializedEvent));

            var items = originalValues.Zip(
                resultValues,
                (o, r) => new
                {
                    o.Name,
                    o.Type,
                    DefaultValue = GetDefaultValue(o.Type),
                    Expected = o.value,
                    Actual = r
                });

            foreach (var item in items)
            {
                if (AreEqual(item.Expected, item.DefaultValue))
                    inconclusiveItems.Add(string.Format("OriginalEvent.{0} == default({1}). This is a poor test value.",
                                                        item.Name, item.Type));
                Assert.That(item.Actual, Is.EqualTo(item.Expected), item.Name);
            }

        }

        protected virtual PropertyInfo[] GetProperties()
        {
            var t = typeof(T);
            return t.GetProperties();
        }


        protected virtual FieldInfo[] GetFields()
        {
            var t = typeof(T);
            return t.GetFields();
        }

        protected virtual bool AreEqual(object o1, object o2)
        {
            if (o1 == null)
                return o2 == null;
            return o1.Equals(o2);
        }

        protected virtual object GetDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }



    }
}

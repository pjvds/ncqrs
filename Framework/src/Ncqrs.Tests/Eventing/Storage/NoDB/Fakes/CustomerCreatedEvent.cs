using System;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Eventing.Storage.NoDB.Tests.Fakes
{
    [Serializable]
    public class CustomerCreatedEvent
    {
        public CustomerCreatedEvent()
        {
        }

        public CustomerCreatedEvent(string name, int age)
        {
            Name = name;
            Age = age;
        }

        public string Name { get; set; }

        public int Age { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as CustomerCreatedEvent;
            if (other == null) return false;
            bool result = Name.Equals(other.Name) &&
                          Age.Equals(other.Age);
            return result;
        }
    }
}
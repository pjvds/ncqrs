using System;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Eventing.Storage.NoDB.Tests.Fakes
{
    [Serializable]
    public class CustomerNameChanged
    {
        public CustomerNameChanged()
        {
        }

        public CustomerNameChanged(Guid customerId, string newName)
        {
            NewName = newName;
            CustomerId = customerId;
        }

        public Guid CustomerId { get; set; }
        public string NewName { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as CustomerNameChanged;
            if (other == null) return false;
            bool result = CustomerId.Equals(other.CustomerId) &&
                          NewName.Equals(other.NewName);
            return result;
        }
    }
}
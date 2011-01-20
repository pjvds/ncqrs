namespace Ncqrs.Eventing.Storage.SQLite.Tests.Fakes{
    using System;
    using Domain;
    using Ncqrs.Eventing.Sourcing;

    [Serializable]
    public class CustomerNameChanged {
        public CustomerNameChanged()
        {
        }
        
        public CustomerNameChanged(string newName)
        {
            NewName = newName;
        }

        public string NewName { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as CustomerNameChanged;
            if (other == null) return false;
            var result = 
                NewName.Equals(other.NewName);
            return result;
        }
    }
}
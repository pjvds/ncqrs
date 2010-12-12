using System;

namespace Ncqrs.Messaging
{
    public class Destination
    {
        private readonly Type _type;
        private readonly Guid _id;

        public Destination(Type type, Guid id)
        {
            _type = type;
            _id = id;
        }

        public Guid Id
        {
            get { return _id; }
        }

        public Type Type
        {
            get { return _type; }
        }
    }
}
using System;

namespace Ncqrs.Spec.Fakes
{

    public class FrozenClock : IClock 
    {
        private readonly DateTime _value;

        public FrozenClock()
            : this(DateTime.UtcNow)
        {
        }

        public FrozenClock(DateTime value)
        {
            _value = value;
        }


        public DateTime UtcNow()
        {
            return _value;
        }
    }

}

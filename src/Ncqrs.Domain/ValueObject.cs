using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ncqrs.Domain
{
    public abstract class ValueObject<T> : IEquatable<T> // TODO: Make this an interface?
    {
        public abstract bool Equals(T other);
    }
}

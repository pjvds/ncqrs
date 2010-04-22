using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ncqrs.Eventing
{
    public class FixedUniqueIdentifierGenerator : IUniqueIdentifierGenerator
    {
        private readonly Guid _fixedValue;
        private Boolean _generationMethodCalled = false;

        public FixedUniqueIdentifierGenerator(Guid fixedValue)
        {
            _fixedValue = fixedValue;
        }

        public Guid GenerateNewId(EventSource eventSource)
        {
            if (_generationMethodCalled) throw new InvalidOperationException("Can only call GenerateNewId once when useing the FixedUniqueIdentifierGenerator.");
            _generationMethodCalled = true;

            return _fixedValue;
        }
    }
}

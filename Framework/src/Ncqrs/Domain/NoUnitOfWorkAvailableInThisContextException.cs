using System;
using System.Runtime.Serialization;

namespace Ncqrs.Domain
{
    [Serializable]
    public class NoUnitOfWorkAvailableInThisContextException : Exception
    {
        public NoUnitOfWorkAvailableInThisContextException() : this("There is no unit of work available in this context.")
        {
        }

        public NoUnitOfWorkAvailableInThisContextException(string message)
            : base(message)
        {
        }

        public NoUnitOfWorkAvailableInThisContextException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected NoUnitOfWorkAvailableInThisContextException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}

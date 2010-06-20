using System;
using System.Runtime.Serialization;

namespace Ncqrs.Eventing.Storage
{
    [Serializable]
    public class PropertyBagConvertionException : Exception
    {
        public PropertyBagConvertionException()
        {
        }

        public PropertyBagConvertionException(string message) : base(message)
        {
        }

        public PropertyBagConvertionException(string message, Exception inner) : base(message, inner)
        {
        }

        protected PropertyBagConvertionException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
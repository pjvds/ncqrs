using System;
using System.Collections.Generic;

namespace Ncqrs.Messaging
{
    //public class TransportMessageMappingRuleEvaluator
    //{
        
    //}

    //public interface ITransportMessageMappingRule
    //{
    //    bool Map(object source, IncomingMessage destination);
    //    Type SourceType { get; }
    //}

    //public class TransportMessageMappingRule<TValue, TSource> : ITransportMessageMappingRule
    //{
    //    private readonly Action<IncomingMessage, TValue> _setAction;
    //    private readonly Func<TSource, TValue> _getFunction;

    //    public TransportMessageMappingRule(Action<IncomingMessage, TValue> setAction, Func<TSource, TValue> getFunction)
    //    {
    //        _setAction = setAction;
    //        _getFunction = getFunction;
    //    }

    //    public bool Map(object source, IncomingMessage destination)
    //    {
    //        _setAction(destination, _getFunction((TSource) source));
    //    }

    //    public Type SourceType
    //    {
    //        get { return typeof (TSource); }
    //    }
    //}

    //public class TransportMessageMappingRuleComparer : IComparer<ITransportMessageMappingRule>
    //{
    //    public int Compare(ITransportMessageMappingRule x, ITransportMessageMappingRule y)
    //    {
    //        if (x.SourceType.IsAssignableFrom(y.SourceType))
    //        {
    //            return 1;
    //        }
    //        if (y.SourceType.IsAssignableFrom(x.SourceType))
    //        {
    //            return -1;
    //        }
    //        return 0;
    //    }
    //}

    public class MappingReceivingStrategy<T> : IReceivingStrategy
    {
        private readonly Func<T, IncomingMessage> _mappingFunction;

        public MappingReceivingStrategy(Func<T, IncomingMessage> mappingFunction)
        {
            _mappingFunction = mappingFunction;
        }

        public IncomingMessage Receive(object message)
        {
            return _mappingFunction((T) message);
        }
    }
}
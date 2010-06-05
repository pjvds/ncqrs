using System;
using System.Text.RegularExpressions;

namespace Ncqrs.Messaging
{
    public class LocalInMemoryReceivingStrategy : IReceivingStrategy
    {
        private const string UrnLocal = "urn:local:{0}/{1}";
        private static readonly Regex _namePattern = new Regex("urn:local:([^/]+)/(.+)", RegexOptions.Compiled);

        public IncomingMessage Receive(object message)
        {
            var typedMessage = (OutgoingMessage) message;

            var match = _namePattern.Match(typedMessage.ReceiverId);
            if (!match.Success)
            {
                throw new InvalidOperationException();
            }
            string typeName = match.Groups[1].Value;
            string id = match.Groups[2].Value;
            return new IncomingMessage
                       {
                           MessageId = typedMessage.MessageId,
                           Payload = typedMessage.Payload,
                           ProcessingRequirements = typedMessage.ProcessingRequirements,
                           ReceiverId = new Guid(id),
                           ReceiverType = Type.GetType(typeName, true),
                           RelatedMessageId = typedMessage.RelatedMessageId,
                           SenderId = MakeId(typedMessage.SenderType, typedMessage.SenderId)
                       };
        }

        public static string MakeId(Type aggregateType, Guid aggregateId)
        {
            return string.Format(UrnLocal, 
                aggregateType.AssemblyQualifiedName, 
                aggregateId);
        }        
    }
}
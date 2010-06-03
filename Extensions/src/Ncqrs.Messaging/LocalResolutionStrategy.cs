using System;
using System.Text.RegularExpressions;

namespace Ncqrs.Messaging
{
   public class LocalResolutionStrategy : IReceiverResolutionStrategy
   {
      private static readonly Regex _namePattern = new Regex("urn:local:([^/]+)/(.+)", RegexOptions.Compiled);

      public ReceiverInfo Resolve(string receiverId)
      {
         var match = _namePattern.Match(receiverId);
         if (!match.Success)
         {
            throw new InvalidOperationException();
         }
         string typeName = match.Groups[1].Value;
         string id = match.Groups[2].Value;
         return new ReceiverInfo(new Guid(id), Type.GetType(typeName, true));
      }

      public static bool Matches(string receiverId)
      {
         return _namePattern.IsMatch(receiverId);
      }

      public static string MakeId(Type aggregateType, Guid aggregateId)
      {
         return string.Format("urn:local:{0}/{1}", aggregateType.AssemblyQualifiedName, aggregateId);
      }
   }
}
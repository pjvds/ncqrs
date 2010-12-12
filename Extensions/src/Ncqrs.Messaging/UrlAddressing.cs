using System;
using System.Text.RegularExpressions;

namespace Ncqrs.Messaging
{
    public class UrlAddressing : IAddressing
    {
        private const string UrnLocal = "urn:typeAndId:{0}/{1}";
        private static readonly Regex _namePattern = new Regex("urn:typeAndId:([^/]+)/(.+)", RegexOptions.Compiled);

        public string EncodeAddress(Destination destination)
        {
            return string.Format(UrnLocal,
                                 destination.Type.AssemblyQualifiedName,
                                 destination.Id);
        }

        public Destination DecodeAddress(string encodedAddress)
        {
            var match = _namePattern.Match(encodedAddress);
            if (!match.Success)
            {
                throw new InvalidOperationException();
            }
            string typeName = match.Groups[1].Value;
            string id = match.Groups[2].Value;

            return new Destination(Type.GetType(typeName, true), new Guid(id));
        }
    }
}
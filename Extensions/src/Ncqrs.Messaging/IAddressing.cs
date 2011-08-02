namespace Ncqrs.Messaging
{
    public interface IAddressing
    {
        string EncodeAddress(Destination destination);
        Destination DecodeAddress(string encodedAddress);
    }
}
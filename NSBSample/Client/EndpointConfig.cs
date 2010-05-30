using NServiceBus;

namespace Client
{
   public class EndpointConfig : IConfigureThisEndpoint, AsA_Server, IWantCustomInitialization
   {
      public void Init()
      {
         Configure.With()
            .DefaultBuilder()
            .BinarySerializer();         
      }
   }
}
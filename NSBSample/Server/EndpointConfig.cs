using System;
using NServiceBus;

namespace Server
{
   public class EndpointConfig : IConfigureThisEndpoint, AsA_Server, IWantCustomInitialization
   {
      public void Init()
      {
         Configure.With()
            .DefaultBuilder()
            .BinarySerializer()
            .InstallNcqrs()
            .UseMappedExecutors();
      }
   }
}
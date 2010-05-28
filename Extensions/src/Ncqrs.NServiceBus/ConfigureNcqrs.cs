using Ncqrs;
using Ncqrs.NServiceBus;

namespace NServiceBus
{
   public static class ConfigureNcqrs
   {
      public static ConfigNcqrs InstallNcqrs(this Configure config)
      {
         var configNcqrs = new ConfigNcqrs();
         configNcqrs.Configure(config);
         return configNcqrs;
      }
   }
}
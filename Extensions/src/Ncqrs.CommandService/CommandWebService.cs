using System;
using System.Diagnostics.Contracts;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Ncqrs.Commanding.ServiceModel;
using Ncqrs.CommandService.Contracts;
using Ncqrs.CommandService.Infrastructure;

namespace Ncqrs.CommandService
{
    [CommandServiceBehavior]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(Namespace = Namespaces.NcqrsCommandWebService, 
                     InstanceContextMode = InstanceContextMode.PerCall)]
    public class CommandWebService : ICommandWebService
    {
        private static ICommandService _service;

        public CommandWebService(ICommandService service)
        {
            _service = service;
        }

        public ExecuteResponse Execute(ExecuteRequest executeRequest)
        {
            Contract.Requires(executeRequest != null);
            Contract.Requires(executeRequest.Command != null);
            Contract.Ensures(Contract.Result<ExecuteResponse>() != null);
            Contract.EnsuresOnThrow<FaultException<CommandWebServiceFault>>(Contract.Result<ExecuteResponse>() == null);

            try
            {
                _service.Execute(executeRequest.Command);
                return new ExecuteResponse();
            }
            catch (Exception ex)
            {

                throw new FaultException<CommandWebServiceFault>(
                    new CommandWebServiceFault(executeRequest.Command, ex), "An exception occured while trying to execute the command");
            }
        }
    }
}

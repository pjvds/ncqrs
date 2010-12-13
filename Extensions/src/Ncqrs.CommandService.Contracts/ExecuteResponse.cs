using System.ServiceModel;

namespace Ncqrs.CommandService.Contracts
{
    [MessageContract(WrapperName = "ExecuteResponse", 
                     WrapperNamespace = Namespaces.NcqrsCommandWebServiceData)]
    public class ExecuteResponse
    {
    }
}

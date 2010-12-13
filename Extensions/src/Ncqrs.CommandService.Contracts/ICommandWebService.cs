using System.ServiceModel;
using Ncqrs.Commanding;

namespace Ncqrs.CommandService.Contracts
{
    /// <summary>
    /// 
    /// </summary>
    [ServiceContract(Namespace = Namespaces.NcqrsCommandWebService)]
    public interface ICommandWebService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        [OperationContract(Action = Namespaces.NcqrsCommandWebService + "ExecuteRequest", 
                           ReplyAction = Namespaces.NcqrsCommandWebService + "ExecuteResponse")]
        ExecuteResponse Execute(ExecuteRequest executeRequest);
    }

    public interface ICommandWebServiceClient : ICommandWebService, IClientChannel { }
}

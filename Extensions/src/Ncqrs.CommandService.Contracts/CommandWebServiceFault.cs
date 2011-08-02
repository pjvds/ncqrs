using System;
using System.Runtime.Serialization;
using Ncqrs.Commanding;

namespace Ncqrs.CommandService.Contracts
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Namespace = Namespaces.NcqrsCommandWebServiceData)]
    public class CommandWebServiceFault
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember(Order = 0)]
        public CommandBase Command { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Order = 1)]
        public string Message { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CommandWebServiceFault()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ex"></param>
        public CommandWebServiceFault(CommandBase command, Exception ex)
        {
            Command = command;
            Message = ex.Message;
        }
    }
}

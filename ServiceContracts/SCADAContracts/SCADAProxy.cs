using OMSCommon;
using OMSSCADACommon.Commands;
using OMSSCADACommon.Responses;
using SCADAContracts;
using System.ServiceModel;

namespace SCADAContracts
{
    public class SCADAProxy : ClientBase<ISCADAContract>, ISCADAContract
    {
        public SCADAProxy() : base(NetTcpBindingCreator.Create(), new EndpointAddress("net.tcp://localhost:4000/SCADAService"))
        {

        }

        public Response ExecuteCommand(Command command)
        {
            return Channel.ExecuteCommand(command);
        }

        public bool Ping()
        {
            return Channel.Ping();
        }
    }
}

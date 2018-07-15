using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using DMSCommon.Model;
using DMSContract;
using IMSContract;
using OMSCommon;

namespace TransactionManagerContract.ClientDMS
{
    public class ClientDMSProxy : ClientBase<IDMSContract>, IDMSContract
    {
        public ClientDMSProxy() : base(NetTcpBindingCreator.Create(), new EndpointAddress("net.tcp://localhost:7000/ClientDMSService"))
        {

        }

        public List<ACLine> GetAllACLines()
        {
            throw new NotImplementedException();
        }

        public List<Consumer> GetAllConsumers()
        {
            throw new NotImplementedException();
        }

        public List<Element> GetAllElements()
        {
            throw new NotImplementedException();
        }

        public List<Node> GetAllNodes()
        {
            throw new NotImplementedException();
        }

        public List<Source> GetAllSources()
        {
            return Channel.GetAllSources();
        }

        public List<Switch> GetAllSwitches()
        {
            throw new NotImplementedException();
        }

        public List<Element> GetNetwork(string mrid)
        {
            return Channel.GetNetwork(mrid);
        }

        public int GetNetworkDepth()
        {
            throw new NotImplementedException();
        }

        public Source GetTreeRoot()
        {
            throw new NotImplementedException();
        }

        public void SendCrew(IncidentReport report, Crew crew)
        {
            throw new NotImplementedException();
        }
    }
}

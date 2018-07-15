using DMSCommon.Model;
using IMSContract;
using OMSCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DMSContract.Proxies
{
    public class DMSProxy : ClientBase<IDMSContract>, IDMSContract
    {
        public DMSProxy() : base(NetTcpBindingCreator.Create(), new EndpointAddress("net.tcp://localhost:5000/DMSService"))
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

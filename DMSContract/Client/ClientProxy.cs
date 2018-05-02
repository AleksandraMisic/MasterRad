using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using DMSCommon.Model;
using OMSCommon;

namespace DMSContract.Client
{
    public class ClientProxy : ClientBase<IClient>, IClient
    {
        public ClientProxy() : base(NetTcpBindingCreator.Create(), new EndpointAddress("net.tcp://localhost:6010/ClientService"))
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
    }
}

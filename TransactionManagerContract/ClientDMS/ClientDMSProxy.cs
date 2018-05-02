using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using DMSCommon.Model;
using OMSCommon;

namespace TransactionManagerContract.ClientDMS
{
    public class ClientDMSProxy : ClientBase<IClientDMS>, IClientDMS
    {
        public ClientDMSProxy() : base(NetTcpBindingCreator.Create(), new EndpointAddress("net.tcp://localhost:6002/ClientDMSService"))
        {

        }

        public List<Source> GetAllSources()
        {
            return Channel.GetAllSources();
        }

        public List<Element> GetNetwork(string mrid)
        {
            return Channel.GetNetwork(mrid);
        }
    }
}

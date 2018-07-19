using FTN.Common;
using FTN.ServiceContracts;
using OMSCommon;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace TransactionManagerContract.ClientNMS
{
    public class ClientNMSProxy : ClientBase<IClientNMS>, IClientNMS
    {
        public ClientNMSProxy() : base(NetTcpBindingCreator.Create(), new EndpointAddress("net.tcp://localhost:7002/ClientNMSService"))
        {

        }

        public ResourceDescription GetStaticDataForElement(long gid)
        {
            return Channel.GetStaticDataForElement(gid);
        }
    }
}

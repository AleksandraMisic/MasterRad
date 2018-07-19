using OMSCommon;
using OMSSCADACommon;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using TransactionManagerContract.ClientSCADA;

namespace TransactionManagerContract.ClientNMS
{
    public class ClientSCADAProxy : ClientBase<IClientSCADA>, IClientSCADA
    {
        public ClientSCADAProxy() : base(NetTcpBindingCreator.Create(), new EndpointAddress("net.tcp://localhost:7003/ClientSCADAService"))
        {

        }

        public void GetAnalogMeasurement(string mrid, out float value)
        {
            Channel.GetAnalogMeasurement(mrid, out value);
        }

        public void GetDiscreteMeasurement(string mrid, out States state)
        {
            Channel.GetDiscreteMeasurement(mrid, out state);
        }
    }
}

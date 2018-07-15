using OMSCommon;
using System;
using System.ServiceModel;

namespace DMSContract
{
    public class DMSCallProxy : ClientBase<IDMSCallContract>, IDMSCallContract
    {
        public DMSCallProxy() : base(NetTcpBindingCreator.Create(), new EndpointAddress("net.tcp://localhost:5003/DMSCallService"))
        {

        }

        public void SendCall(string mrid)
        {
            Channel.SendCall(mrid);
        }
    }
}

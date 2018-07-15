using OMSCommon;
using OMSSCADACommon;
using System;
using System.ServiceModel;

namespace DMSContract
{
    public class DMSSCADAProxy : ClientBase<IDMSSCADAContract>, IDMSSCADAContract
    {
        public DMSSCADAProxy() : base(NetTcpBindingCreator.Create(), new EndpointAddress("net.tcp://localhost:5002/DMSSCADAService"))
        {

        }

        public void ChangeOnSCADAAnalog(string mrID, float value)
        {
            Channel.ChangeOnSCADAAnalog(mrID, value);
        }

        public void ChangeOnSCADADigital(string mrID, States state)
        {
            Channel.ChangeOnSCADADigital(mrID, state);
        }
    }
}

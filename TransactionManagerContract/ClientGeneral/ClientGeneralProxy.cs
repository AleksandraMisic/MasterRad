using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using DMSCommon.Model;
using DMSContract;
using FTN.Common;
using IMSContract;
using OMSCommon;
using OMSSCADACommon;
using TransactionManagerContract.ClientGeneral;

namespace TransactionManagerContract
{
    public class ClientGeneralProxy : ClientBase<IClientGeneral>, IClientGeneral
    {
        public ClientGeneralProxy() : base(NetTcpBindingCreator.Create(), new EndpointAddress("net.tcp://localhost:6001/ClientGeneralService"))
        {

        }

        public void Temp()
        {
            throw new NotImplementedException();
        }
    }
}

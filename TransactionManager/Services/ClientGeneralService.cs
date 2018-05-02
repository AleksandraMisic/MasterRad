using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMSCommon.Model;
using DMSContract.Client;
using FTN.Common;
using TransactionManagerContract;
using TransactionManagerContract.ClientGeneral;

namespace TransactionManager.ServiceImplementations
{
    public class ClientGeneralService : IClientGeneral
    {
        private ClientProxy clientProxy;

        public ClientGeneralService()
        {
            clientProxy = new ClientProxy();
        }

        public void Temp()
        {
            throw new NotImplementedException();
        }
    }
}

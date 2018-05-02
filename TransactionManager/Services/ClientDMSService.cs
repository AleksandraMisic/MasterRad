using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMSCommon.Model;
using DMSContract.Client;
using TransactionManagerContract.ClientDMS;

namespace TransactionManager.Services
{
    public class ClientDMSService : IClientDMS
    {
        private ClientProxy clientProxy;

        public ClientDMSService()
        {
            clientProxy = new ClientProxy();
        }

        public List<Source> GetAllSources()
        {
            List<Source> listOfDMSSources = new List<Source>();
            try
            {
                listOfDMSSources = clientProxy.GetAllSources();
            }
            catch (Exception e) { }

            return listOfDMSSources;
        }

        public List<Element> GetNetwork(string mrid)
        {
            List<Element> listOfDMSElements = new List<Element>();
            try
            {
                listOfDMSElements = clientProxy.GetNetwork(mrid);
            }
            catch (Exception e) { }

            return listOfDMSElements;
        }
    }
}

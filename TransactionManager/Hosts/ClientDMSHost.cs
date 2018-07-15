using DMSContract;
using OMSCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using TransactionManager.Services;
using TransactionManagerContract.ClientDMS;

namespace TransactionManager.Hosts
{
    public class ClientDMSHost
    {
        private ServiceHost svc = null;

        public void Start()
        {
            svc = new ServiceHost(typeof(ClientDMSService));
            svc.AddServiceEndpoint(typeof(IDMSContract), NetTcpBindingCreator.Create(), new Uri("net.tcp://localhost:7000/ClientDMSService"));

            svc.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            svc.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

            svc.Open();
            Console.WriteLine("ClientDMSService ready and waiting for requests.");
        }
        public void Stop()
        {
            svc.Close();
            Console.WriteLine("ClientDMSService has stopped.");
        }
    }
}

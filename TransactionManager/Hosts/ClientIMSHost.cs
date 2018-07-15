using IMSContract;
using OMSCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace TransactionManager.Services
{
    public class ClientIMSHost
    {
        private ServiceHost svc = null;

        public void Start()
        {
            svc = new ServiceHost(typeof(ClientIMSHost));
            svc.AddServiceEndpoint(typeof(IIMSContract), NetTcpBindingCreator.Create(), new Uri("net.tcp://localhost:7001/ClientIMSService"));

            svc.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            svc.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

            svc.Open();
            Console.WriteLine("ClientIMSService ready and waiting for requests.");
        }
        public void Stop()
        {
            svc.Close();
            Console.WriteLine("ClientIMSService has stopped.");
        }
    }
}

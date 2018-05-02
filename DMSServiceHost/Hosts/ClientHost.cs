using DMSContract;
using DMSContract.Client;
using DMSService;
using OMSCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace DMSServiceHost.Hosts
{
    public class ClientHost
    {
        private ServiceHost svc = null;

        public void Start()
        {
            svc = new ServiceHost(typeof(ClientService));
            svc.AddServiceEndpoint(typeof(IClient), NetTcpBindingCreator.Create(), new Uri("net.tcp://localhost:6010/ClientService"));

            svc.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            svc.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

            svc.Open();
            Console.WriteLine("ClientServiceHost ready and waiting for requests.");
        }
        public void Stop()
        {
            svc.Close();
            Console.WriteLine("ClientServiceHost server stopped.");
        }
    }
}

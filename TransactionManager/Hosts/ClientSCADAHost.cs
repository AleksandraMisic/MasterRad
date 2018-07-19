using OMSCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using TransactionManager.Services;
using TransactionManagerContract.ClientSCADA;

namespace TransactionManager.Hosts
{
    public class ClientSCADAHost
    {
        private ServiceHost svc = null;

        public void Start()
        {
            svc = new ServiceHost(typeof(ClientSCADAService));
            svc.AddServiceEndpoint(typeof(IClientSCADA), NetTcpBindingCreator.Create(), new Uri("net.tcp://localhost:7003/ClientSCADAService"));

            svc.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            svc.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

            svc.Open();
            Console.WriteLine("ClientISCADAService is ready and waiting for requests.");
        }
        public void Stop()
        {
            svc.Close();
            Console.WriteLine("ClientISCADAService has stopped.");
        }
    }
}

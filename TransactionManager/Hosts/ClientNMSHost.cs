using OMSCommon;
using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using TransactionManager.Services;
using TransactionManagerContract.ClientNMS;

namespace TransactionManager.Hosts
{
    public class ClientNMSHost
    {
        private ServiceHost svc = null;

        public void Start()
        {
            svc = new ServiceHost(typeof(ClientNMSService));
            svc.AddServiceEndpoint(typeof(IClientNMS), NetTcpBindingCreator.Create(), new Uri("net.tcp://localhost:7002/ClientNMSService"));

            svc.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            svc.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

            svc.Open();
            Console.WriteLine("ClientNMSService ready and waiting for requests.");
        }

        public void Stop()
        {
            svc.Close();
            Console.WriteLine("ClientNMSService has stopped.");
        }
    }
}

using OMSCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using TransactionManager.ServiceImplementations;
using TransactionManagerContract.ClientGeneral;

namespace TransactionManager.Hosts
{
    public class ClientGeneralHost
    {
        private ServiceHost svc = null;

        public void Start()
        {
            svc = new ServiceHost(typeof(ClientGeneralService));
            svc.AddServiceEndpoint(typeof(IClientGeneral), NetTcpBindingCreator.Create(), new Uri("net.tcp://localhost:6001/ClientGeneralService"));

            svc.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            svc.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

            svc.Open();
            Console.WriteLine("ClientGeneralService ready and waiting for requests.");
        }
        public void Stop()
        {
            svc.Close();
            Console.WriteLine("ClientGeneralService server stopped.");
        }
    }
}

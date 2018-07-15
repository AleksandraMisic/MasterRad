using OMSCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using TransactionManagerContract;

namespace TransactionManager.Hosts
{
    public class DistributedTransactionHost
    {
        private ServiceHost svc = null;

        public void Start()
        {
            svc = new ServiceHost(typeof(DistributedTransactionService));
            svc.AddServiceEndpoint(typeof(IDistributedTransaction), NetTcpBindingCreator.Create(), new Uri("net.tcp://localhost:7002/DistributedTransactionService"));

            svc.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            svc.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

            svc.Open();
            Console.WriteLine("DistributedTransactionService ready and waiting for requests.");
        }
        public void Stop()
        {
            svc.Close();
            Console.WriteLine("DistributedTransactionService server stopped.");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MITM_Common.MITM_Service
{
    public class MITM_ServiceManager
    {
        private ServiceHost svc = null;

        public void Start()
        {
            svc = new ServiceHost(typeof(MITM_Service));

            svc.AddServiceEndpoint(typeof(IMITM_Contract), NetTcpBindingCreator.Create(), new Uri("net.tcp://localhost:7023/MITM_Service"));
            
            svc.Open();
            Console.WriteLine("MITM Service ready and waiting for requests.");
        }
        public void Stop()
        {
            svc.Close();
            Console.WriteLine("MITM Service has stopped.");
        }
    }
}

using MITM_Common;
using MITM_Common.MITM_Service;
using MITM_Common.PubSub;
using PubSub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MITM_Service
{
    class Program
    {
        private static ServiceHost publishServiceHost = null;
        private static ServiceHost subscribeServiceHost = null;

        static void Main(string[] args)
        {
            try
            {
                HostPublishService();
                HostSubscribeService();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine(" -----------------STARTED-----------------\n\n\n\n");

            Console.Title = "MITM Service";
            MITM_ServiceManager msm = new MITM_ServiceManager();

            GetConnectionInfo getConnectionInfo = new GetConnectionInfo();

            TaskFactory taskFactory = new TaskFactory();
            taskFactory.StartNew(() => getConnectionInfo.Get());

            msm.Start();

            Console.ReadLine();

            msm.Stop();
        }

        private static void HostPublishService()
        {
            publishServiceHost = new ServiceHost(typeof(PublishService));

            publishServiceHost.AddServiceEndpoint(typeof(IPublisher), NetTcpBindingCreator.Create(), "net.tcp://localhost:7021/PublishService");
            publishServiceHost.Open();
        }

        private static void HostSubscribeService()
        {
            subscribeServiceHost = new ServiceHost(typeof(SubscriberService));

            subscribeServiceHost.AddServiceEndpoint(typeof(ISubscriber), NetTcpBindingCreator.Create(), "net.tcp://localhost:7020/SubscribeService");
            subscribeServiceHost.Open();
        }
    }
}

using OMSCommon;
using PubSubContract;
using PubSubscribeService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PubSubscribeService
{
    class Program
    {
        private static ServiceHost publishServiceHost = null;
        private static ServiceHost subscribeServiceHost = null;

        static void Main(string[] args)
        {
            Console.Title = "Publisher-Subscribe";
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

            Console.WriteLine(" Press any key to STOP services");
            Console.ReadLine();
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("-------------------------------------");
            Console.ReadLine();
        }

        private static void HostPublishService()
        {
            publishServiceHost.AddServiceEndpoint(typeof(IPublishing), NetTcpBindingCreator.Create(), "net.tcp://localhost:3001/Pub");
            publishServiceHost.Open();
        }

        private static void HostSubscribeService()
        {
            subscribeServiceHost.AddServiceEndpoint(typeof(ISubscription), NetTcpBindingCreator.Create(), "net.tcp://localhost:3002/Sub");
            subscribeServiceHost.Open();
        }
    }
}

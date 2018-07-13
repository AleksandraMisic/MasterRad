using MITM_Common;
using MITM_Common.MITM_Service;
using MITM_Common.PubSub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PubSub
{
    public class Publisher : IPublisher
    {
        IPublisher proxy;

        public Publisher()
        {
            CreateProxy();
        }

        public void ReturnConnectionInfo(GlobalConnectionInfo connectionInfo)
        {
            try
            {
                proxy.ReturnConnectionInfo(connectionInfo);
            }
            catch { }
        }

        private void CreateProxy()
        {
            string address = "";
            try
            {
                address = "net.tcp://localhost:7021/PublishService";
                EndpointAddress endpointAddress = new EndpointAddress(address);
                proxy = ChannelFactory<IPublisher>.CreateChannel(NetTcpBindingCreator.Create(), endpointAddress);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}

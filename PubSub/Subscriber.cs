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
    public delegate void PublishConnectionInfoEvent(ConnectionInfoStruct connectionInfoStruct);

    public class Subscriber : IPublisher
    { 
        ISubscriber proxy = null;

        public event PublishConnectionInfoEvent publishConnectionInfoEvent;

        public Subscriber()
        {
            CreateProxy();
        }

        public void ReturnConnectionInfo(ConnectionInfoStruct connectionInfo)
        {
            publishConnectionInfoEvent?.Invoke(connectionInfo);
        }

        public void Subscribe()
        {
            try
            {
                proxy.Subscribe();
            }
            catch (Exception e)
            {

            }
        }

        public void UnSubscribe()
        {
            try
            {
                proxy.UnSubscribe();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void CreateProxy()
        {
            try
            {  
                EndpointAddress endpointAddress = new EndpointAddress("net.tcp://localhost:7020/SubscribeService");
                InstanceContext callback = new InstanceContext(this);
                DuplexChannelFactory<ISubscriber> channelFactory = new DuplexChannelFactory<ISubscriber>(callback, NetTcpBindingCreator.Create(), endpointAddress);
                proxy = channelFactory.CreateChannel();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}

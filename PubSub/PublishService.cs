using MITM_Common.MITM_Service;
using MITM_Common.PubSub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PubSub
{
    public class PublishService : IPublisher
    {
        public void ReturnConnectionInfo(ConnectionInfoStruct connectionInfo)
        {
            foreach (IPublisher subscriber in PubSubDatabase.Subscribers)
            {
                PublishThreadData threadObj = new PublishThreadData(subscriber, connectionInfo);

                Thread thread = new Thread(() => threadObj.PublishConnectionInfo(connectionInfo));
                thread.Start();
            }
        }
    }

    internal class PublishThreadData
    {
        private IPublisher subscriber;

        private ConnectionInfoStruct connectionInfo;
        
        public PublishThreadData(IPublisher subscriber, ConnectionInfoStruct connectionInfo)
        {
            this.subscriber = subscriber;
            this.connectionInfo = connectionInfo;
        }

        public IPublisher Subscriber
        {
            get
            {
                return subscriber;
            }

            set
            {
                subscriber = value;
            }
        }

        public ConnectionInfoStruct ConnectionInfoStruct
        {
            get
            {
                return connectionInfo;
            }

            set
            {
                connectionInfo = value;
            }
        }

        public void PublishConnectionInfo(ConnectionInfoStruct connectionInfo)
        {
            try
            {
                subscriber.ReturnConnectionInfo(connectionInfo);
            }
            catch (Exception e)
            {
                PubSubDatabase.RemoveSubsriber(subscriber);
            }
        }
    }
}

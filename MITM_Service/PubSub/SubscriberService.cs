using MITM_Common;
using MITM_Common.PubSub;
using MITM_Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PubSub
{
    public class SubscriberService : ISubscriber
    {
        Publisher publisher = null;

        public SubscriberService()
        {
            publisher = new Publisher();
        }

        public void Subscribe()
        {
            IPublisher subscriber = OperationContext.Current.GetCallbackChannel<IPublisher>();
            PubSubDatabase.AddSubscriber(subscriber);

            //publisher.ReturnConnectionInfo(Database.GlobalConnectionInfo);
        }

        public void UnSubscribe()
        {
            IPublisher subscriber = OperationContext.Current.GetCallbackChannel<IPublisher>();
            PubSubDatabase.RemoveSubsriber(subscriber);
        }
    }
}

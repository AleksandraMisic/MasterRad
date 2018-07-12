using MITM_Common.PubSub;
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
        public void Subscribe()
        {
            IPublisher subscriber = OperationContext.Current.GetCallbackChannel<IPublisher>();
            PubSubDatabase.AddSubscriber(subscriber);
        }

        public void UnSubscribe()
        {
            IPublisher subscriber = OperationContext.Current.GetCallbackChannel<IPublisher>();
            PubSubDatabase.RemoveSubsriber(subscriber);
        }
    }
}

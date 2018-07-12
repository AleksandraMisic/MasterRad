using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITM_Common.PubSub
{
    public class PubSubDatabase
    {
        private static object locker = new object();

        private static List<IPublisher> subscribers = new List<IPublisher>();

        public static List<IPublisher> Subscribers
        {
            get
            {
                return subscribers;
            }
        }

        public static void AddSubscriber(IPublisher subscriber)
        {
            lock (locker)
            {
                try
                {
                    Subscribers.Add(subscriber);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public static void RemoveSubsriber(IPublisher subscriber)
        {
            lock (locker)
            {
                Subscribers.Remove(subscriber);
            }
        }
    }
}

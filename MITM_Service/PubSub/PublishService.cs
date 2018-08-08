using DNP3DataPointsModel;
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
        public void AnalogInputChange(AnalogInputPoint analogInputPoint)
        {
            foreach (IPublisher subscriber in PubSubDatabase.Subscribers)
            {
                PublishThreadData threadObj = new PublishThreadData(subscriber, analogInputPoint);

                Thread thread = new Thread(() => threadObj.PublishAnalogInputChangeInfo(analogInputPoint));
                thread.Start();
            }
        }

        public void ReturnConnectionInfo(GlobalConnectionInfo connectionInfo)
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

        private GlobalConnectionInfo connectionInfo;
        private AnalogInputPoint analogInputPoint;
        
        public PublishThreadData(IPublisher subscriber, GlobalConnectionInfo connectionInfo)
        {
            this.subscriber = subscriber;
            this.connectionInfo = connectionInfo;
        }

        public PublishThreadData(IPublisher subscriber, AnalogInputPoint analogInputPoint)
        {
            this.subscriber = subscriber;
            this.analogInputPoint = analogInputPoint;
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

        public GlobalConnectionInfo ConnectionInfoStruct
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

        public void PublishConnectionInfo(GlobalConnectionInfo connectionInfo)
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

        public void PublishAnalogInputChangeInfo(AnalogInputPoint analogInputPoint)
        {
            try
            {
                subscriber.AnalogInputChange(analogInputPoint);
            }
            catch (Exception e)
            {
                PubSubDatabase.RemoveSubsriber(subscriber);
            }
        }
    }
}

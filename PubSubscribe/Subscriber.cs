using DMSCommon;
using IMSContract;
using OMSCommon;
using OMSSCADACommon;
using PubSubContract;
using System;
using System.Collections.Generic;
using System.ServiceModel;


namespace PubSubscribe
{
    public delegate void PublishDigitalUpdateEvent(string mrid, States state);
    public delegate void PublishAnalogUpdateEvent(string mrid, float value);
    public delegate void PublishCrewEvent(UIUpdateModel update);
    public delegate void PublishReportIncident(IncidentReport report);
    public delegate void PublishCallIncident(UIUpdateModel call);
    public delegate void PublishUIBreakers(bool IsIncident,long incidentBreaker);

    /// <summary>
    /// Client for Subscribing service
    /// </summary>
    public class Subscriber : IPublishing
    {
        ISubscription subscriptionProxy = null;

        public event PublishDigitalUpdateEvent publishDigitalUpdateEvent;
        public event PublishAnalogUpdateEvent publishAnalogUpdateEvent;
        public event PublishCrewEvent publishCrewEvent;
        public event PublishReportIncident publishIncident;
        public event PublishCallIncident publishCall;
        public event PublishUIBreakers publiesBreakers;

        public Subscriber()
        {
            CreateProxy();
        }

        private void CreateProxy()
        {
            try
            {  
                EndpointAddress endpointAddress = new EndpointAddress("net.tcp://localhost:3002/Sub");
                InstanceContext callback = new InstanceContext(this);
                DuplexChannelFactory<ISubscription> channelFactory = new DuplexChannelFactory<ISubscription>(callback, NetTcpBindingCreator.Create(), endpointAddress);
                subscriptionProxy = channelFactory.CreateChannel();
            }
            catch (Exception e)
            {
                throw e;
                //TODO  Log error : PubSub not started
            }
        }

        public void Subscribe()
        {
            try
            {
                subscriptionProxy.Subscribe();
            }
            catch (Exception e)
            {
                //throw e;
                //TODO  Log error 
            }

        }

        public void UnSubscribe()
        {
            try
            {
                subscriptionProxy.UnSubscribe();
            }
            catch (Exception e)
            {
                throw e;
                //TODO  Log error 
            }
        }

        public void PublishDigitalUpdate(string mrid, States state)
        {
            publishDigitalUpdateEvent?.Invoke(mrid, state);
        }

        public void PublishAnalogUpdate(string mrid, float value)
        {
            publishAnalogUpdateEvent?.Invoke(mrid, value);
        }

        public void PublishCrewUpdate(UIUpdateModel update)
        {
            publishCrewEvent?.Invoke(update);
        }

        public void PublishIncident(IncidentReport report)
        {
            publishIncident?.Invoke(report);
        }

        public void PublishCallIncident(UIUpdateModel call)
        {
            publishCall?.Invoke(call);
        }

        public void PublishUIBreakers(bool IsIncident, long incidentBreaker)
        {
            publiesBreakers?.Invoke(IsIncident, incidentBreaker);
        }       
    }
}

using DMSCommon;
using IMSContract;
using OMSCommon;
using OMSSCADACommon;
using PubSubContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PubSubscribe
{
    /// <summary>
    /// Client for Publishing service
    /// </summary>
    public class Publisher : IPublishing
    {
        IPublishing proxy;

        public Publisher()
        {
            CreateProxy();
        }

        // not used
        public void PublishCrew(UIUpdateModel update)
        {
            try
            {
                proxy.PublishCrewUpdate(update);
            }
            catch { }
        }

        public void PublishIncident(IncidentReport report)
        {
            try
            {
                proxy.PublishIncident(report);
            }
            catch { }
        }

        public void PublishCallIncident(UIUpdateModel call)
        {
            try
            {
                proxy.PublishCallIncident(call);
            }
            catch { }
        }

        public void PublishUIBreaker(bool isIncident,long incidentBreaker)
        {
            try
            {
                proxy.PublishUIBreakers(isIncident, incidentBreaker);
            }
            catch { }
        }

        private void CreateProxy()
        {
            string address = "";
            try
            {
                address = "net.tcp://localhost:3001/Pub";
                EndpointAddress endpointAddress = new EndpointAddress(address);
                proxy = ChannelFactory<IPublishing>.CreateChannel(NetTcpBindingCreator.Create(), endpointAddress);
            }
            catch (Exception e)
            {
                throw e;
                //TODO log error;
            }

        }

        public void PublishDigitalUpdate(string mrid, States state)
        {
            try
            {
                proxy.PublishDigitalUpdate(mrid, state);
            }
            catch (Exception e) { }
        }

        public void PublishAnalogUpdate(string mrid, float value)
        {
            try
            {
                proxy.PublishAnalogUpdate(mrid, value);
            }
            catch { }
        }

        public void PublishCrewUpdate(UIUpdateModel update)
        {
            throw new NotImplementedException();
        }

        public void PublishUIBreakers(bool isIncident, long incidentBreaker)
        {
            throw new NotImplementedException();
        }

        public void PublishEnergizationChange(List<UIUpdateModel> update)
        {
            try
            {
                proxy.PublishEnergizationChange(update);
            }
            catch { }
        }
    }
}

using DMSCommon;
using IMSContract;
using OMSSCADACommon;
using System.Collections.Generic;
using System.ServiceModel;

namespace PubSubContract
{
    [ServiceContract]
    public interface IPublishing
    {
        [OperationContract(IsOneWay = true)]
        void PublishDigitalUpdate(string mrid, States state);

        [OperationContract(IsOneWay = true)]
        void PublishAnalogUpdate(string mrid, float value);

        [OperationContract(IsOneWay = true)]
        void PublishCrewUpdate(UIUpdateModel update);

        [OperationContract(IsOneWay = true)]
        void PublishIncident(IncidentReport report);

        [OperationContract(IsOneWay = true)]
        void PublishCallIncident(UIUpdateModel call);

        [OperationContract(IsOneWay = true)]
        void PublishUIBreakers(bool isIncident,long incidentBreaker);
    }
}

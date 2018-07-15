using DMSCommon.Model;
using IMSContract;
using System.Collections.Generic;
using System.ServiceModel;

namespace DMSContract
{
    [ServiceContract]
    public interface IDMSContract
    {
        [OperationContract]
        List<Source> GetAllSources();

        [OperationContract]
        List<Consumer> GetAllConsumers();

        [OperationContract]
        List<Switch> GetAllSwitches();

        [OperationContract]
        List<ACLine> GetAllACLines();

        [OperationContract]
        List<Node> GetAllNodes();

        [OperationContract]
        Source GetTreeRoot();

        [OperationContract]
        int GetNetworkDepth();

        [OperationContract]
        List<Element> GetAllElements();

        [OperationContract]
        List<Element> GetNetwork(string mrid);

        [OperationContract]
        void SendCrew(IncidentReport report, Crew crew);
    }
}

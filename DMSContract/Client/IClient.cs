using DMSCommon.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DMSContract.Client
{
    [ServiceContract]
    public interface IClient
    {
        [OperationContract]
        List<Element> GetNetwork(string mrid);

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
        int GetNetworkDepth();

        [OperationContract]
        Source GetTreeRoot();
    }
}

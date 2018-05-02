using DMSCommon.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TransactionManagerContract.ClientDMS
{
    [ServiceContract]
    public interface IClientDMS
    {
        [OperationContract]
        List<Element> GetNetwork(string mrid);

        [OperationContract]
        List<Source> GetAllSources();
    }
}

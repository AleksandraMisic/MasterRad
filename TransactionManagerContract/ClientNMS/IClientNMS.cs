using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TransactionManagerContract.ClientNMS
{
    [ServiceContract]
    public interface IClientNMS
    {
        [OperationContract]
        ResourceDescription GetStaticDataForElement(long gid);
    }
}

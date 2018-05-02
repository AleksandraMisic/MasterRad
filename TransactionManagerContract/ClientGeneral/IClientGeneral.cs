using DMSCommon.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TransactionManagerContract.ClientGeneral
{
    [ServiceContract]
    public interface IClientGeneral
    {
        [OperationContract]
        void Temp();
    }
}

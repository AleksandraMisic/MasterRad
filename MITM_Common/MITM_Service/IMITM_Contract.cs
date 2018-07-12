using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MITM_Common.MITM_Service
{
    [ServiceContract]
    public interface IMITM_Contract
    {
        [OperationContract]
        void Temp();
    }
}

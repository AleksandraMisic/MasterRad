using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MITM_Common.PubSub
{
    [ServiceContract(CallbackContract = typeof(IPublisher))]
    public interface ISubscriber
    {
        [OperationContract]
        void Subscribe();

        [OperationContract]
        void UnSubscribe();
    }
}

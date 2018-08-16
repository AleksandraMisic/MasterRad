using DNP3TCPDriver.UserLevel;
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
        List<Host> SniffForHosts();

        [OperationContract]
        void ARPSpoof(ARPSpoofParticipantsInfo participants);

        [OperationContract]
        void TerminateActiveAttack();

        [OperationContract]
        void FixValue(PointType pointType, int index);

        [OperationContract]
        void ReleaseValue(PointType pointType, int index);
    }
}

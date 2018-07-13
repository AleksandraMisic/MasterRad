using MITM_Common;
using MITM_Common.MITM_Service;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace MITM_Common
{
    public class MITMServiceProxy : ChannelFactory<IMITM_Contract>, IMITM_Contract, IDisposable
    {
        private IMITM_Contract factory;

        public MITMServiceProxy(NetTcpBinding binding, string address = "net.tcp://localhost:7023/MITM_Service") : base(binding, address)
        {
            factory = this.CreateChannel();
        }

        public MITMServiceProxy(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }

        public void ARPSpoof(ARPSpoofParticipantsInfo participants)
        {
            try
            {
                factory.ARPSpoof(participants);
            }
            catch (Exception e)
            {
                
            }
        }

        public List<Host> SniffForHosts()
        {
            try
            {
                return factory.SniffForHosts();
            }
            catch (Exception e)
            {
                return new List<Host>();
            }
        }

        public void TerminateActiveAttack()
        {
            try
            {
                factory.TerminateActiveAttack();
            }
            catch (Exception e)
            {
                
            }
        }
    }
}

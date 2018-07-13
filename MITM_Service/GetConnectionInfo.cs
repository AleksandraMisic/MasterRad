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
    public class GetConnectionInfo
    {
        [DllImport("ARPSpoof.dll", EntryPoint = "GetNetworkInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetNetworkInfo(ref ConnectionInfoStruct name);

        Publisher publisher = null;
        
        private const int isConnectedSleep = 500;

        public GetConnectionInfo()
        {
            publisher = new Publisher();
        }

        public void Get()
        {
            bool change = false;

            while (true)
            {
                ConnectionInfoStruct connectionInfo = new ConnectionInfoStruct() { MACAddress = new byte[6], IPAddress = new byte[4], SubnetMask = new byte[4] };

                GetNetworkInfo(ref connectionInfo);

                if (connectionInfo.IsConnected == 0)
                {
                    if (Database.GlobalConnectionInfo.ConnectionState != ConnectionState.DISCONNECTED)
                    {
                        change = true;
                        Database.GlobalConnectionInfo.ConnectionState = ConnectionState.DISCONNECTED;
                    }

                    if (change)
                    {
                        publisher.ReturnConnectionInfo(Database.GlobalConnectionInfo);
                    }

                    change = false;

                    Thread.Sleep(isConnectedSleep);
                    continue;
                }
                else 
                {
                    if (Database.GlobalConnectionInfo.ConnectionState != ConnectionState.CONNECTED)
                    {
                        change = true;
                        Database.GlobalConnectionInfo.ConnectionState = ConnectionState.CONNECTED;
                    }
                }

                Database.GlobalConnectionInfo.ConnectionState = ConnectionState.CONNECTED;

                if (Database.GlobalConnectionInfo.Name != connectionInfo.Name)
                {
                    change = true;
                    Database.GlobalConnectionInfo.Name = connectionInfo.Name;
                }
                if (Database.GlobalConnectionInfo.Description != connectionInfo.Description)
                {
                    change = true;
                    Database.GlobalConnectionInfo.Description = connectionInfo.Description;
                }
                if (Database.GlobalConnectionInfo.FriendlyName != connectionInfo.FriendlyName)
                {
                    change = true;
                    Database.GlobalConnectionInfo.FriendlyName = connectionInfo.FriendlyName;
                }
                for (int i = 0; i < 4; i++)
                {
                    if (Database.GlobalConnectionInfo.IPAddress[i] != connectionInfo.IPAddress[i])
                    {
                        change = true;
                        Database.GlobalConnectionInfo.IPAddress = connectionInfo.IPAddress;
                        break;
                    }
                }
                for (int i = 0; i < 6; i++)
                {
                    if (Database.GlobalConnectionInfo.MACAddress[i] != connectionInfo.MACAddress[i])
                    {
                        change = true;
                        Database.GlobalConnectionInfo.MACAddress = connectionInfo.MACAddress;
                    }
                }
                for (int i = 0; i < 4; i++)
                {
                    if (Database.GlobalConnectionInfo.SubnetMask[i] != connectionInfo.SubnetMask[i])
                    {
                        change = true;
                        Database.GlobalConnectionInfo.SubnetMask = connectionInfo.SubnetMask;
                    }
                }

                if (change)
                {
                    publisher.ReturnConnectionInfo(Database.GlobalConnectionInfo);
                }

                change = false;

                Thread.Sleep(isConnectedSleep);
            }
        }
    }
}

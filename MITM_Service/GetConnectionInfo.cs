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

        private GlobalConnectionInfo globalConnectionInfo;
        private const int isConnectedSleep = 500;

        public GetConnectionInfo()
        {
            this.globalConnectionInfo = new GlobalConnectionInfo();
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
                    if (this.globalConnectionInfo.ConnectionState != ConnectionState.DISCONNECTED)
                    {
                        change = true;
                        this.globalConnectionInfo.ConnectionState = ConnectionState.DISCONNECTED;
                    }

                    if (change)
                    {
                        Publisher publisher = new Publisher();
                        publisher.ReturnConnectionInfo(this.globalConnectionInfo);
                    }

                    change = false;

                    Thread.Sleep(isConnectedSleep);
                    continue;
                }
                else 
                {
                    if (this.globalConnectionInfo.ConnectionState != ConnectionState.CONNECTED)
                    {
                        change = true;
                        this.globalConnectionInfo.ConnectionState = ConnectionState.CONNECTED;
                    }
                }

                this.globalConnectionInfo.ConnectionState = ConnectionState.CONNECTED;

                if (this.globalConnectionInfo.Name != connectionInfo.Name)
                {
                    change = true;
                    this.globalConnectionInfo.Name = connectionInfo.Name;
                }
                if (this.globalConnectionInfo.Description != connectionInfo.Description)
                {
                    change = true;
                    this.globalConnectionInfo.Description = connectionInfo.Description;
                }
                if (this.globalConnectionInfo.FriendlyName != connectionInfo.FriendlyName)
                {
                    change = true;
                    this.globalConnectionInfo.FriendlyName = connectionInfo.FriendlyName;
                }
                for (int i = 0; i < 4; i++)
                {
                    if (this.globalConnectionInfo.IPAddress[i] != connectionInfo.IPAddress[i])
                    {
                        change = true;
                        this.globalConnectionInfo.IPAddress = connectionInfo.IPAddress;
                        break;
                    }
                }
                for (int i = 0; i < 6; i++)
                {
                    if (this.globalConnectionInfo.MACAddress[i] != connectionInfo.MACAddress[i])
                    {
                        change = true;
                        this.globalConnectionInfo.MACAddress = connectionInfo.MACAddress;
                    }
                }
                for (int i = 0; i < 4; i++)
                {
                    if (this.globalConnectionInfo.SubnetMask[i] != connectionInfo.SubnetMask[i])
                    {
                        change = true;
                        this.globalConnectionInfo.SubnetMask = connectionInfo.SubnetMask;
                    }
                }

                if (change)
                {
                    Publisher publisher = new Publisher();
                    publisher.ReturnConnectionInfo(this.globalConnectionInfo);
                }

                change = false;

                Thread.Sleep(isConnectedSleep);
            }
        }
    }
}

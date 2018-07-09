using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MITM_UI.Model
{
    public class ConnectionInfoClass
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct ConnectionInfoStruct
        {
            public int IsConnected;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
            public string Name;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
            public string Description;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
            public string FriendlyName;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] IPAddress;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] MACAddress;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] DefaultGateway;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] SubnetMask;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 200)]
            public byte[] Hosts;

            public int HostCount;

            public int RoutingEnabled;

            public int Sleep;
        }
    }
}

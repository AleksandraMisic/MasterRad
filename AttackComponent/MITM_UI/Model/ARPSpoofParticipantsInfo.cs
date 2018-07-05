using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MITM_UI.Model
{
    public class ARPSpoofParticipantsInfo
    {
        public struct ARPSpoofParticipantsInfoStruct
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
            public string Name;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] MyIPAddress;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] MyMACAddress;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] Target1IPAddress;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] Target1MACAddress;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] Target2IPAddress;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] Target2MACAddress;
        }
    }
}

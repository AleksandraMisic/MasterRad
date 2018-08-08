using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MITM_Common.MITM_Service
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct SendPacketStruct
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string Name;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 150)]
        public byte[] Packet;

        public int Size;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MITM_Common.MITM_Service
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct PacketStruct
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] source_addr;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1000)]
        public byte[] packet;
        
        public int dataOffset;

        public int dataLength;
    }
}

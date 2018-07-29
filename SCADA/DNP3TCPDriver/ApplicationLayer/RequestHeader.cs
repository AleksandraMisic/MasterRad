using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3TCPDriver.ApplicationLayer
{
    public class RequestHeader : Header, IByteable
    {
        public override byte[] ToBytes()
        {
            byte[] header = new byte[2];
            ApplicationControl.CopyTo(header, 0);
            header[1] = (byte)FunctionCode;

            return header;
        }
    }
}

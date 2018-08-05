using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3TCPDriver.ApplicationLayer
{
    public class ResponseHeader : Header
    {
        public BitArray InternalIndications { get; set; }

        public ResponseHeader()
        {
            InternalIndications = new BitArray(16);
        }

        public override byte[] ToBytes()
        {
            byte[] header = new byte[4];
            ApplicationControl.CopyTo(header, 0);
            header[1] = (byte)FunctionCode;
            InternalIndications.CopyTo(header, 2);

            return header;
        }
    }
}

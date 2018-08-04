using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3TCPDriver.ApplicationLayer
{
    public abstract class Header : IByteable
    {
        public BitArray ApplicationControl { get; set; }

        public ApplicationFunctionCodes FunctionCode { get; set; }

        public Header()
        {
            ApplicationControl = new BitArray(8);
        }

        public abstract byte[] ToBytes();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3TCPDriver.ApplicationLayer
{
    public class RequestHeader
    {
        public BitArray ApplicationControl { get; set; }

        public FunctionCodes FunctionCode { get; set; }

        public BitArray InternalIndications { get; set; }

        public RequestHeader()
        {
            ApplicationControl = new BitArray(8);
            InternalIndications = new BitArray(16);
        }
    }
}

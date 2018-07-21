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

        public ApplicationFunctionCodes FunctionCode { get; set; }

        public RequestHeader()
        {
            ApplicationControl = new BitArray(8);
        }
    }
}

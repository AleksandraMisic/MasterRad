using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3TCPDriver.TransportFunction
{
    public class TransportHeader
    {
        public BitArray Header { get; set; }

        public TransportHeader()
        {
            Header = new BitArray(8);
        }
    }
}

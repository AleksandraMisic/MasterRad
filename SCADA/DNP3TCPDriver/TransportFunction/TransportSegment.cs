using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3TCPDriver.TransportFunction
{
    public class TransportSegment
    {
        public TransportHeader TransportHeader { get; set; }

        public Byte[] Data { get; set; }

        public TransportSegment(TransportHeader transportHeader)
        {
            TransportHeader = transportHeader;
        }
    }
}

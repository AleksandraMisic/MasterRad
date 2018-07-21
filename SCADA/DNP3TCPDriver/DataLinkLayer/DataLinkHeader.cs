using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3TCPDriver.DataLinkLayer
{
    public class DataLinkHeader
    {
        public Byte[] Start { get; set; }
        public Byte Length { get; set; }
        public BitArray Control { get; set; }
        public Byte[] Destination { get; set; }
        public Byte[] Source { get; set; }

        public DataLinkHeader()
        {
            Start = new byte[2];
            Control = new BitArray(8);
            Destination = new byte[2];
            Source = new byte[2];
        }
    }
}

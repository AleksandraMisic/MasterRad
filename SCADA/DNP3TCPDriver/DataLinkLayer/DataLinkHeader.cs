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

        public byte[] GetBytes()
        {
            byte[] array = new byte[2 + 1 + 1 + 2 + 2];

            array[0] = Start[0];
            array[1] = Start[1];
            array[2] = Length;
            Control.CopyTo(array, 3);
            array[4] = Destination[0];
            array[5] = Destination[1];
            array[6] = Source[0];
            array[7] = Source[1];

            return array;
        }
    }
}

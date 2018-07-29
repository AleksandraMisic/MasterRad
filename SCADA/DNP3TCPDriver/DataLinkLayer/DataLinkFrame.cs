using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3TCPDriver.DataLinkLayer
{
    public class DataLinkFrame
    {
        public DataLinkHeader DataLynkHeader { get; set; }
        public Byte[] Data{ get; set; }

        public DataLinkFrame(DataLinkHeader dataLinkHeader)
        {
            DataLynkHeader = dataLinkHeader ?? throw new ArgumentNullException();
        }
    }
}

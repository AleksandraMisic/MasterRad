using DNP3TCPDriver.ApplicationLayer;
using DNP3TCPDriver.DataLinkLayer;
using DNP3TCPDriver.TransportFunction;
using PCCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3TCPDriver
{
    public class DNP3Handler : IIndustryProtocolHandler
    {
        public DNP3ApplicationHandler DNP3ApplicationHandler { get; set; }

        public DNP3TransportFunctionHandler DNP3TransportFunctionHandler { get; set; }

        public DNP3DataLinkHandler DNP3DataLynkHandler { get; set; }

        public DNP3Handler()
        {
            DNP3ApplicationHandler = new DNP3ApplicationHandler();
            DNP3TransportFunctionHandler = new DNP3TransportFunctionHandler();
            DNP3DataLynkHandler = new DNP3DataLinkHandler();
        }

        public byte[] PackData()
        {
            throw new NotImplementedException();
        }

        public void UnpackData(byte[] data, int length)
        {
            throw new NotImplementedException();
        }
    }
}

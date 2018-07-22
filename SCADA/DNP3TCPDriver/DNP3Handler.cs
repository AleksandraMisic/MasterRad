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
        public DNP3DataLinkHandler DNP3DataLinkHandler { get; set; }

        public DNP3Handler()
        {
            DNP3ApplicationHandler = new DNP3ApplicationHandler();
            DNP3DataLinkHandler = new DNP3DataLinkHandler();
        }

        public byte[] PackData()
        {
            return DNP3ApplicationHandler.DNP3TransportFunctionHandler.DNP3DataLinkHandler.PackedFrame;
        }

        public void UnpackData(byte[] data, int length)
        {
            throw new NotImplementedException();
        }
    }
}

using DNP3TCPDriver.DataLynkLayer;
using PCCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3TCPDriver.DataLinkLayer
{
    public class DNP3DataLinkHandler : IIndustryProtocolHandler, IDataLinkHandler
    {
        public DataLinkFrame DataLinkFrame { get; set; }

        public DNP3DataLinkHandler()
        {
            DataLinkFrame = new DataLinkFrame();
        }

        public byte[] PackData()
        {
            throw new NotImplementedException();
        }

        public void UnpackData(byte[] data, int length)
        {
            throw new NotImplementedException();
        }

        public void MakeFrame(byte[] data)
        {
            DataLinkHeader dataLinkHeader = new DataLinkHeader();

            dataLinkHeader.Start[0] = 0x05;
            dataLinkHeader.Start[1] = 0x64;

            dataLinkHeader.Length = BitConverter.GetBytes(5 + data.Count())[0];

            dataLinkHeader.Control[0] = true;
            dataLinkHeader.Control[1] = true;
            dataLinkHeader.Control[2] = false;
            dataLinkHeader.Control[3] = false;

            BitArray bitArray = new BitArray((byte)DataLinkFunctionCodes.UNCONFIRMED_USER_DATA);

            dataLinkHeader.Control[4] = bitArray[4];
            dataLinkHeader.Control[5] = bitArray[5];
            dataLinkHeader.Control[6] = bitArray[6];
            dataLinkHeader.Control[7] = bitArray[7];

            dataLinkHeader.Destination[0] = 1;
            dataLinkHeader.Destination[1] = 1;

            dataLinkHeader.Source[0] = 1;
            dataLinkHeader.Source[1] = 1;

            DataLinkFrame.DataLynkHeader = dataLinkHeader;

            int totalSize = 5 + data.Count();
        }
    }
}

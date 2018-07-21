using DNP3TCPDriver.DataLinkLayer;
using DNP3TCPDriver.DataLynkLayer;
using PCCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3TCPDriver.TransportFunction
{
    public class DNP3TransportFunctionHandler : IIndustryProtocolHandler, ITransportFunctionHandler
    {
        public DNP3DataLinkHandler DNP3DataLinkHandler { get; set; }

        private int segmentMaxSize = 249;

        public List<TransportSegment> TransportSegments { get; set; }

        public TransportSegment CurrentSegment { get; set; }

        public DNP3TransportFunctionHandler()
        {
            DNP3DataLinkHandler = new DNP3DataLinkHandler();
            TransportSegments = new List<TransportSegment>();
            CurrentSegment = new TransportSegment();
        }

        public byte[] PackData()
        {
            byte[] array = new byte[CurrentSegment.Data.Count() + 1];

            CurrentSegment.TransportHeader.Header.CopyTo(array, 0);
            CurrentSegment.Data.CopyTo(array, 1);

            return array;
        }

        public void UnpackData(byte[] data, int length)
        {
            throw new NotImplementedException();
        }

        public void MakeSegments(byte[] array)
        {
            if (array.Count() > segmentMaxSize)
            {

            }
            else
            {
                TransportSegment transportSegment = new TransportSegment();
                TransportHeader transportHeader = new TransportHeader();

                transportHeader.Header[7] = true;       // FIN
                transportHeader.Header[6] = true;       // FIR
                transportHeader.Header[5] = false;      // Sequence
                transportHeader.Header[4] = false;
                transportHeader.Header[3] = false;
                transportHeader.Header[3] = false;
                transportHeader.Header[2] = false;
                transportHeader.Header[0] = false;

                transportSegment.TransportHeader = transportHeader;
                transportSegment.Data = array;

                TransportSegments.Add(transportSegment);

                CurrentSegment = transportSegment;

                DNP3DataLinkHandler.MakeFrame(PackData());
            }
        }
    }
}

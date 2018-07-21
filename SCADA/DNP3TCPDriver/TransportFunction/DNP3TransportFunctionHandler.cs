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
        private IDataLinkHandler dataLinkHandler;

        private int segmentMaxSize = 249;

        public List<TransportSegment> TransportSegments { get; set; }

        public TransportSegment CurrentSegment { get; set; }

        public DNP3TransportFunctionHandler()
        {
            TransportSegments = new List<TransportSegment>();
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
            dataLinkHandler = new DNP3DataLinkHandler();

            if (array.Count() > segmentMaxSize)
            {

            }
            else
            {
                TransportSegment transportSegment = new TransportSegment();
                TransportHeader transportHeader = new TransportHeader();

                transportHeader.Header[0] = true;
                transportHeader.Header[1] = true;
                transportHeader.Header[2] = false;
                transportHeader.Header[3] = false;
                transportHeader.Header[4] = false;
                transportHeader.Header[5] = false;
                transportHeader.Header[6] = false;
                transportHeader.Header[7] = false;

                transportSegment.TransportHeader = transportHeader;
                transportSegment.Data = array;

                TransportSegments.Add(transportSegment);

                CurrentSegment = transportSegment;

                ((DNP3DataLinkHandler)dataLinkHandler).MakeFrame(PackData());
            }
        }
    }
}

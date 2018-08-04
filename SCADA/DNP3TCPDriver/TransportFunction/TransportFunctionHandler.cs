using DNP3TCPDriver.ApplicationLayer;
using DNP3TCPDriver.DataLinkLayer;
using PCCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3TCPDriver.TransportFunction
{
    public class TransportFunctionHandler
    {
        public DataLinkHandler DNP3DataLinkHandler { get; set; }
        public ApplicationHandler DNP3ApplicationHandler { get; set; }

        private int segmentMaxSize = 249;

        private bool Fir = false;
        private bool Fin = false;

        private byte sequence = 0;

        public void PackUp(byte[] data)
        {
            List<byte[]> appSegments = new List<byte[]>();

            BitArray header = new BitArray(new byte[] { data[0] });
            BitArray tempHeader = new BitArray(header);

            tempHeader[0] = false;
            tempHeader[1] = false;

            byte[] tempSeq = new byte[1];
            tempHeader.CopyTo(tempSeq, 0);

            byte newSeq = tempSeq[0];

            if (header[0] == true)
            {
                Fir = true;
                sequence = newSeq;
            }
            else if (!Fir)
            {
                return;
            }
            else if(newSeq != sequence)
            {
                return;
            }

            sequence++;

            Fin = header[1];

            int length = data.Count() - 1;

            byte[] appData = new byte[length];

            for (int i = 0; i < length; i++)
            {
                appData[i] = data[i + 1];
            }

            appSegments.Add(appData);

            if (Fin)
            {
                Fir = false;

                int totalLength = 0;
                foreach (byte[] segment in appSegments)
                {
                    totalLength += segment.Count();
                }

                byte[] finalAppData = new byte[totalLength];

                int index = 0;
                foreach (byte[] segment in appSegments)
                {
                    segment.CopyTo(finalAppData, index);
                    index += segment.Count();
                }

                DNP3ApplicationHandler = new ApplicationHandler();
                DNP3ApplicationHandler.PackUp(finalAppData);
            }
        }

        public List<byte[]> PackDown(byte[] data, bool isRequest, bool isMaster)
        {
            List<byte[]> segments = new List<byte[]>();

            bool fir = true;
            byte sequence = 0;

            TransportHeader transportHeader = new TransportHeader();
            TransportSegment transportSegment = new TransportSegment(transportHeader);

            int dataCount = data.Count();
            int dataIndex = 0;

            while (dataCount > 0)
            {
                if (sequence == 64)
                {
                    sequence = 0;
                }
                else
                {
                    sequence++;
                }

                int totalLength = 1;

                byte[] temp = null;

                if (fir)
                {
                    transportHeader.Header[6] = true;       // FIR
                    fir = false;
                    sequence = 0;
                }
                else
                {
                    transportHeader.Header[6] = false;       // FIR
                }

                if (dataCount <= segmentMaxSize)
                {
                    transportHeader.Header[7] = true;       // FIN

                    temp = new byte[dataCount + 1];

                    for (int i = 1; i <= dataCount; i++)
                    {
                        temp[i] = data[dataIndex++];
                    }

                    totalLength += dataCount;
                }
                else
                {
                    temp = new byte[segmentMaxSize + 1];

                    for (int i = 1; i <= dataCount; i++)
                    {
                        temp[i] = data[dataIndex++];
                    }

                    totalLength += segmentMaxSize;
                }

                dataCount -= segmentMaxSize;
                dataIndex += segmentMaxSize;

                BitArray tempSeq = new BitArray(new byte[] { sequence });
                
                transportHeader.Header[5] = tempSeq[5];      // Sequence
                transportHeader.Header[4] = tempSeq[4];
                transportHeader.Header[3] = tempSeq[3];
                transportHeader.Header[3] = tempSeq[2];
                transportHeader.Header[2] = tempSeq[1];
                transportHeader.Header[0] = tempSeq[0];

                temp[0] = transportHeader.ToBytes()[0];

                byte[] finalDataLinkSegment = new byte[totalLength];

                for (int i = 0; i < totalLength; i++)
                {
                    finalDataLinkSegment[i] = temp[i];
                }

                segments.Add(finalDataLinkSegment);
            }

            List<byte[]> returnValue = new List<byte[]>();
            DNP3DataLinkHandler = new DataLinkHandler();

            foreach (byte[] segment in segments)
            {
                List<byte[]> tempSegments =new List<byte[]>() { DNP3DataLinkHandler.PackDown(segment, isRequest, isMaster, DataLinkFunctionCodes.UNCONFIRMED_USER_DATA) };

                foreach (byte[] tempSegment in tempSegments)
                {
                    returnValue.Add(tempSegment);
                }
            }

            return returnValue;
        }
    }
}

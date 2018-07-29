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
    public class TransportFunctionHandler : IProtocolLayer
    {
        public DataLinkHandler DNP3DataLinkHandler { get; set; }
        public ApplicationHandler DNP3ApplicationHandler { get; set; }

        private int segmentMaxSize = 249;

        private List<byte[]> AppSegments;
        private Queue<byte[]> DataLinkSegments;

        private bool IsRequest;

        private bool Fir = false;
        private bool Fin = false;

        private byte sequence = 0;

        public TransportFunctionHandler(bool isRequest)
        {
            AppSegments = new List<byte[]>();
            DataLinkSegments = new Queue<byte[]>();

            IsRequest = isRequest;
        }

        public void PackUp(byte[] data)
        {
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
                AppSegments.Clear();
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

            AppSegments.Add(appData);

            if (Fin)
            {
                Fir = false;

                int totalLength = 0;
                foreach (byte[] segment in AppSegments)
                {
                    totalLength += segment.Count();
                }

                byte[] finalAppData = new byte[totalLength];

                int index = 0;
                foreach (byte[] segment in AppSegments)
                {
                    segment.CopyTo(finalAppData, index);
                    index += segment.Count();
                }

                DNP3ApplicationHandler = new ApplicationHandler();
                DNP3ApplicationHandler.PackUp(finalAppData);
            }
        }

        public void PackDown(byte[] data)
        {
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

                DataLinkSegments.Enqueue(finalDataLinkSegment);
            }
        }
    }
}

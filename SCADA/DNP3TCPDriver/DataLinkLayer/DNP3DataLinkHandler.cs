using DNP3TCPDriver.DataLynkLayer;
using DNP3TCPDriver.TransportFunction;
using PCCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DNP3TCPDriver.DataLinkLayer
{
    public class DNP3DataLinkHandler : IIndustryProtocolHandler, IDataLinkHandler
    {
        [DllImport("CRCCalculator.dll", EntryPoint = "CalculateCRC", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CalculateCRC(int length, byte[] data, byte[] crc);

        public DNP3TransportFunctionHandler DNP3TransportFunctionHandler;

        public DataLinkFrame DataLinkFrame { get; set; }

        public byte[] PackedFrame { get; set; }

        public DNP3DataLinkHandler()
        {
            //DNP3TransportFunctionHandler = new DNP3TransportFunctionHandler();
            DataLinkFrame = new DataLinkFrame();
        }

        public byte[] PackData()
        {
            throw new NotImplementedException();
        }

        public void UnpackData(byte[] data, int length)
        {
            if (data[0] != 0x05 || data[1] != 0x64)
            {
                return;
            }

            int actualTransportLen = 0;

            byte[] temp = new byte[length];

            length -= 8; // minus header
            length -= 2; // minus header crc

            int i = 10, j = 0;

            while (length > 0)
            {
                if (length < 18)
                {
                    // last chunk
                    actualTransportLen += (byte)(length - 2);

                    int k = 0;
                    for (k = 0; k < length - 2; k++)
                    {
                        temp[j + k] = data[i + k];
                    }

                    i += k + 2;
                    j += k;
                    break;
                }

                for (int k = 0; k < 16; k++)
                {
                    temp[j + k] = data[i + k];
                }

                i += 16;
                j += 18;

                actualTransportLen += (byte)(16);
                length -= 18;
            }

            byte[] transportMessage = new byte[actualTransportLen];

            for (i = 0; i < actualTransportLen; i++)
            {
                transportMessage[i] = temp[i];
            }

            DNP3TransportFunctionHandler = new DNP3TransportFunctionHandler();
            DNP3TransportFunctionHandler.UnpackData(transportMessage, actualTransportLen);
        }

        public void MakeFrame(byte[] data, bool isRequest)
        {
            DataLinkHeader dataLinkHeader = new DataLinkHeader();

            dataLinkHeader.Start[0] = 0x05;
            dataLinkHeader.Start[1] = 0x64;

            dataLinkHeader.Length = BitConverter.GetBytes(5 + data.Count())[0];

            if (isRequest)
            {
                dataLinkHeader.Control[7] = true;       // DIR
                dataLinkHeader.Control[6] = true;       // PRM
            }
            else
            {
                dataLinkHeader.Control[7] = false;       // DIR
                dataLinkHeader.Control[6] = false;       // PRM
            }
            dataLinkHeader.Control[5] = false;      // FCB
            dataLinkHeader.Control[4] = false;      // FCV

            BitArray bitArray = new BitArray(new Byte[] { (byte)DataLinkFunctionCodes.UNCONFIRMED_USER_DATA });

            dataLinkHeader.Control[3] = bitArray[3];    // Function code
            dataLinkHeader.Control[2] = bitArray[2];
            dataLinkHeader.Control[1] = bitArray[1];
            dataLinkHeader.Control[0] = bitArray[0];

            dataLinkHeader.Destination[0] = 1;
            dataLinkHeader.Destination[1] = 1;

            dataLinkHeader.Source[0] = 2;
            dataLinkHeader.Source[1] = 2;

            DataLinkFrame.DataLynkHeader = dataLinkHeader;

            byte[] array = new byte[dataLinkHeader.GetBytes().Count() + 2 + data.Count() + 2];
            byte[] header = new byte[dataLinkHeader.GetBytes().Count()];

            dataLinkHeader.GetBytes().CopyTo(array, 0);
            dataLinkHeader.GetBytes().CopyTo(header, 0);

            byte[] crc = CallCalclulateCRC(header);
            
            array[8] = crc[0];   // Header CRC
            array[9] = crc[1];

            int dataCount = data.Count();
            int dataIndex = 0;
            int arrayIndex = 10;

            while (dataCount > 0)
            {
                byte[] temp = new byte[16];

                if (dataCount < 16)
                {
                    byte[] lastTemp = new byte[dataCount];
                    data.CopyTo(lastTemp, dataIndex);
                    lastTemp.CopyTo(array, arrayIndex);

                    byte[] crc1 = CallCalclulateCRC(lastTemp);

                    arrayIndex += dataCount;
                    array[arrayIndex++] = crc1[0];
                    array[arrayIndex++] = crc1[1];

                    break;
                }

                dataIndex += 16;
                dataCount -= 16;
            }

            PackedFrame = array;
        }

        private byte[] CallCalclulateCRC(byte[] data)
        {
            byte[] crc = new byte[2];

            string path = Directory.GetCurrentDirectory();

            try
            {
                CalculateCRC(data.Count(), data, crc);
            }
            catch (Exception e) { }

            return crc;
        }
    }
}

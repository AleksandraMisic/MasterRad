using DNP3TCPDriver.TransportFunction;
using DNP3TCPDriver.UserLevel;
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
    public class DataLinkHandler
    {
        [DllImport("CRCCalculator.dll", EntryPoint = "CalculateCRC", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CalculateCRC(int length, byte[] data, byte[] crc);

        public TransportFunctionHandler DNP3TransportFunctionHandler;

        public bool IsMaster { get; set; }
        public bool IsPrm { get; set; }

        private int frameMaxSize = 282;

        public List<UserLevelObject> PackUp(byte[] data)
        {
            if (data[0] != 0x05 || data[1] != 0x64)
            {
                return null;
            }

            BitArray ctrl = new BitArray(new byte[1] { data[3]});

            if (ctrl[7])
            {
                IsMaster = true;
            }
            else
            {
                IsMaster = false;
            }
            if (ctrl[6])
            {
                IsPrm = true;
            }
            else
            {
                IsPrm = false;
            }

            int length = data.Count();

            int actualTransportLength = 0;

            byte[] temp = new byte[length];

            length -= 8; // minus header
            length -= 2; // minus header crc

            int i = 10, j = 0;

            while (length > 0)
            {
                if (length < 18)
                {
                    // last chunk
                    actualTransportLength += (byte)(length - 2);

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

                actualTransportLength += (byte)(16);
                length -= 18;
            }

            byte[] transportMessage = new byte[actualTransportLength];

            for (i = 0; i < actualTransportLength; i++)
            {
                transportMessage[i] = temp[i];
            }

            DNP3TransportFunctionHandler = new TransportFunctionHandler();
            return DNP3TransportFunctionHandler.PackUp(transportMessage);
        }

        public byte[] PackDown(byte[] data, bool direction, bool primary, DataLinkFunctionCodes functionCode)
        {
            DataLinkHeader dataLinkHeader = new DataLinkHeader();

            dataLinkHeader.Start[0] = 0x05;
            dataLinkHeader.Start[1] = 0x64;

            dataLinkHeader.Length = BitConverter.GetBytes(5 + data.Count())[0];

            dataLinkHeader.Control[7] = direction;       // DIR (Master/Outstation)
            dataLinkHeader.Control[6] = primary;       // PRM (Initiated/Completed)

            dataLinkHeader.Control[5] = false;      // FCB
            dataLinkHeader.Control[4] = false;      // FCV

            BitArray bitArray = new BitArray(new Byte[] { (byte)functionCode });

            dataLinkHeader.Control[3] = bitArray[3];    // Function code
            dataLinkHeader.Control[2] = bitArray[2];
            dataLinkHeader.Control[1] = bitArray[1];
            dataLinkHeader.Control[0] = bitArray[0];

            dataLinkHeader.Destination[0] = 1;
            dataLinkHeader.Destination[1] = 1;

            dataLinkHeader.Source[0] = 2;
            dataLinkHeader.Source[1] = 2;

            byte[] header = dataLinkHeader.ToBytes();
            byte[] tempFrame = new byte[frameMaxSize]; // HARD CODE!!!!!!!!!!!!!!!!

            header.CopyTo(tempFrame, 0);

            byte[] crc = CallCalclulateCRC(header);

            tempFrame[8] = crc[0];   // Header CRC
            tempFrame[9] = crc[1];

            int dataCount = data.Count();
            int dataIndex = 0;
            int frameIndex = 10;

            while (dataCount > 0)
            {
                byte[] temp = null;

                if (dataCount < 16)
                {
                    temp = new byte[dataCount];
                    data.CopyTo(temp, dataIndex);
                    temp.CopyTo(tempFrame, frameIndex);

                    byte[] crc1 = CallCalclulateCRC(temp);

                    frameIndex += dataCount;
                    tempFrame[frameIndex++] = crc1[0];
                    tempFrame[frameIndex++] = crc1[1];

                    break;
                }

                temp = new byte[16];

                data.CopyTo(temp, dataIndex);
                temp.CopyTo(tempFrame, frameIndex);

                byte[] crc2 = CallCalclulateCRC(temp);

                frameIndex += 16;
                tempFrame[frameIndex++] = crc2[0];
                tempFrame[frameIndex++] = crc2[1];

                dataIndex += 16;
                dataCount -= 16;
            }

            byte[] frame = new byte[frameIndex];

            for (int i = 0; i < frameIndex; i++)
            {
                frame[i] = tempFrame[i];
            }

            return frame;
        }

        private byte[] CallCalclulateCRC(byte[] data)
        {
            byte[] crc = new byte[2];

            string path = Directory.GetCurrentDirectory();

            try
            {
                CalculateCRC(data.Count(), data, crc);
            }
            catch (Exception e)
            {
                
            }

            return crc;
        }
    }
}

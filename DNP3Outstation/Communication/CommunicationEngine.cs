using DNP3Driver.ApplicationLayer;
using DNP3Outstation.ObjectProcessing;
using DNP3TCPDriver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DNP3Outstation.Communication
{
    public class CommunicationEngine
    {
        TcpListener server = null;

        private DNP3Handler DNP3Handler;

        public CommunicationEngine(string address, int port)
        {
            DNP3Handler = new DNP3Handler();

            IPAddress ipAddress = IPAddress.Parse(address);

            server = new TcpListener(ipAddress, port);
            server.Start();

            Task.Factory.StartNew(() => ProcessRequests());
        }

        void ProcessRequests()
        {
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Task.Factory.StartNew(() => ProcessRequest(client));
            }
        }

        void ProcessRequest(TcpClient client)
        {
            Byte[] bytes = new Byte[1000];
            
            NetworkStream stream = client.GetStream();

            byte actualLen = 0;

            int i = 0;
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                byte len = bytes[2];

                actualLen = (byte)(2 + 1 + 5 + 2); // start + len + ctrl + dest + source + crc

                len -= 5; // minus header

                while (len > 0)
                {
                    if (len < 16)
                    {
                        // last chunk
                        actualLen += (byte)(len + 2);
                        break;
                    }

                    actualLen += (byte)(16 + 2);
                    len -= 16;
                }

                byte[] message = new byte[actualLen];

                for (i = 0; i < actualLen; i++)
                {
                    message[i] = bytes[i];
                }

                //DNP3Handler.DNP3DataLinkHandler.UnpackData(message, actualLen);

                DissectRequest();

                //DNP3Handler.DNP3DataLinkHandler.DNP3TransportFunctionHandler.DNP3ApplicationHandler.ReadAllAnalogInputPointsResponse();
                
                int offset = 0;

                try
                {
                    //stream.Write(DNP3Handler.DNP3DataLinkHandler.DNP3TransportFunctionHandler.DNP3ApplicationHandler.DNP3TransportFunctionHandler.DNP3DataLinkHandler.PackedFrame, offset, DNP3Handler.DNP3DataLinkHandler.DNP3TransportFunctionHandler.DNP3ApplicationHandler.DNP3TransportFunctionHandler.DNP3DataLinkHandler.PackedFrame.Count());
                }
                catch (Exception e) { }
            }
        }

        void DissectRequest()
        {
            foreach (DNP3Object dnp3Object in DNP3Handler.DNP3DataLinkHandler.DNP3TransportFunctionHandler.DNP3ApplicationHandler.DNP3Objects)
            {
                ObjectProcessor objectProcessor = new ObjectProcessor(dnp3Object);
                objectProcessor.ProcessObject();
            }
        }
    }
}

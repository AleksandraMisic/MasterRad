using DNP3Driver.ApplicationLayer;
using DNP3Outstation.DNP3UserLayer;
using DNP3TCPDriver;
using DNP3TCPDriver.DataLinkLayer;
using DNP3TCPDriver.UserLevel;
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
            DataLinkHandler dataLinkHandler = new DataLinkHandler();

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
                
                List<UserLevelObject> userLevelObjects = dataLinkHandler.PackUp(message);

                if (userLevelObjects == null)
                {
                    continue;
                }

                DNP3Handler dNP3Handler = new DNP3Handler();
                DNP3UserLayerHandler userLayer = new DNP3UserLayerHandler(dNP3Handler);
                List<byte[]> segments = userLayer.ReadAllAnalogInputPointsResponse(userLevelObjects);

                int offset = 0;

                foreach (byte[] segment in segments)
                {
                    try
                    {
                        stream.Write(segment, offset, segment.Count());
                    }
                    catch (Exception e) { }
                }
            }
        }
    }
}

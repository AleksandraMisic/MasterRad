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

        public CommunicationEngine(string address, int port)
        {
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
            int i = 0;
            NetworkStream stream = client.GetStream();

            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
            }
        }
    }
}

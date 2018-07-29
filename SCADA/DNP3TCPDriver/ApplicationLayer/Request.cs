using DNP3Driver.ApplicationLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3TCPDriver.ApplicationLayer
{
    public class Request : IByteable
    {
        public RequestHeader RequestHeader { get; set; }
        public Dictionary<ObjectHeader, List<DNP3Object>> Objects { get; set; }

        public Request()
        {
            RequestHeader = new RequestHeader();
            Objects = new Dictionary<ObjectHeader, List<DNP3Object>>();
        }

        public byte[] ToBytes()
        {
            int totalSize = RequestHeader.ToBytes().Count();

            foreach (ObjectHeader objectHeader in Objects.Keys)
            {
                totalSize += objectHeader.ToBytes().Count();

                foreach (DNP3Object dnp3Object in Objects[objectHeader])
                {
                    totalSize += dnp3Object.ToBytes().Count();
                }
            }

            byte[] array = new byte[totalSize];
            RequestHeader.ToBytes().CopyTo(array, 0);

            int i = 2;
            foreach (ObjectHeader objectHeader in Objects.Keys)
            {
                byte[] temp = objectHeader.ToBytes();
                temp.CopyTo(array, i);

                i += temp.Count();

                foreach (DNP3Object dnp3Object in Objects[objectHeader])
                {
                    temp = dnp3Object.ToBytes();
                    temp.CopyTo(array, i);

                    i += temp.Count();
                }
            }

            return array;
        }
    }
}

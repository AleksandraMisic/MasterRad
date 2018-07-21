using DNP3TCPDriver.TransportFunction;
using PCCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3TCPDriver.ApplicationLayer
{
    public class DNP3ApplicationHandler : IIndustryProtocolHandler, IDNP3ApplicationHandler
    {
        ITransportFunctionHandler transportFunctionHandler;

        public Request Request { get; set; }

        public DNP3ApplicationHandler()
        {
            transportFunctionHandler = new DNP3TransportFunctionHandler();
            Request = new Request();
        }

        public byte[] PackData()
        {
            return Request.GetRequestInBytes();
        }

        public void UnpackData(byte[] data, int length)
        {
            throw new NotImplementedException();
        }

        public void ReadAllAnalogInputPoints(int[] indices)
        {
            RequestHeader requestHeader = new RequestHeader();

            requestHeader.ApplicationControl[0] = true;
            requestHeader.ApplicationControl[1] = true;
            requestHeader.ApplicationControl[2] = false;
            requestHeader.ApplicationControl[3] = false;

            requestHeader.FunctionCode = ApplicationFunctionCodes.READ;

            Request.RequestHeader = requestHeader;

            ObjectHeader objectHeader = new ObjectHeader()
            {
                Group = 30,
                Variation = 3
            };

            objectHeader.QualifierField[0] = false;
            objectHeader.QualifierField[1] = false;
            objectHeader.QualifierField[2] = false;
            objectHeader.QualifierField[3] = false;

            objectHeader.QualifierField[4] = false;
            objectHeader.QualifierField[5] = false;
            objectHeader.QualifierField[6] = false;
            objectHeader.QualifierField[7] = true;

            int startIndex = indices[0], stopIndex = 0;

            foreach (int index in indices)
            {
                startIndex = index;

                if (index == -1)
                {
                    break;
                }
            }

            objectHeader.RangeField = new Byte[2];
            objectHeader.RangeField[0] = BitConverter.GetBytes(startIndex)[0];
            objectHeader.RangeField[1] = BitConverter.GetBytes(stopIndex)[0];

            Request.ObjectHeaders.Add(objectHeader);

            ((DNP3TransportFunctionHandler)transportFunctionHandler).MakeSegments(PackData());
        }
    }
}

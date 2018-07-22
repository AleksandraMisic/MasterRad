using DNP3Driver.ApplicationLayer;
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
        public DNP3TransportFunctionHandler DNP3TransportFunctionHandler { get; set; }

        public List<DNP3Object> DNP3Objects = new List<DNP3Object>();

        private bool packRequest = true;

        public Request Request { get; set; }
        public Response Response { get; set; }

        public DNP3ApplicationHandler()
        {
            //DNP3TransportFunctionHandler = new DNP3TransportFunctionHandler();
            Request = new Request();
            Response = new Response();
        }

        public byte[] PackData()
        {
            if (packRequest)
            {
                return Request.GetRequestInBytes();
            }
            
            return Response.GetResponseInBytes();
        }

        public void UnpackData(byte[] data, int length)
        {
            bool indicesIncluded = false;
            bool startStopIndex = false;
            bool objectCount = false;
            int numOfOctetsInRangeSpec = 0;

            RequestHeader requestHeader = new RequestHeader();
            requestHeader.ApplicationControl = new BitArray(new byte[] { data[0] });
            requestHeader.FunctionCode = (ApplicationFunctionCodes)data[1];

            length -= 2;

            int index = 2;
            while (length > 0)
            {
                length -= 3;

                DNP3Object dNP3Object = new DNP3Object();

                ObjectHeader objectHeader = new ObjectHeader();
                objectHeader.Group = data[index++];
                objectHeader.Variation = data[index++];
                objectHeader.QualifierField = new BitArray(new byte[] { data[index++] });

                dNP3Object.Group = objectHeader.Group;
                dNP3Object.Variation = objectHeader.Variation;

                byte[] qualifierField = new byte[1];
                objectHeader.QualifierField.CopyTo(qualifierField, 0);

                BitArray objectPrefix = new BitArray(3);

                objectPrefix[0] = objectHeader.QualifierField[4];
                objectPrefix[1] = objectHeader.QualifierField[5];
                objectPrefix[2] = objectHeader.QualifierField[6];

                byte[] objectPrefixByte = new byte[1];
                objectPrefix.CopyTo(objectPrefixByte, 0);

                BitArray rangeSpecifier = new BitArray(4);

                rangeSpecifier[0] = objectHeader.QualifierField[0];
                rangeSpecifier[1] = objectHeader.QualifierField[1];
                rangeSpecifier[2] = objectHeader.QualifierField[2];
                rangeSpecifier[3] = objectHeader.QualifierField[3];

                byte[] rangeSpecifierByte = new byte[1];
                rangeSpecifier.CopyTo(rangeSpecifierByte, 0);

                switch (objectPrefixByte[0])
                {
                    case (byte)0x00:
                        indicesIncluded = false;
                        break;
                    case (byte)0x01:
                        indicesIncluded = true;
                        break;
                    case (byte)0x02:
                        indicesIncluded = true;
                        break;
                    case (byte)0x05:
                        indicesIncluded = false;
                        break;
                }

                switch (rangeSpecifierByte[0])
                {
                    case (byte)0x00:
                        startStopIndex = true;
                        objectCount = false;
                        numOfOctetsInRangeSpec = 1;
                        break;
                    case (byte)0x01:
                        startStopIndex = true;
                        objectCount = false;
                        numOfOctetsInRangeSpec = 2;
                        break;
                    case (byte)0x06:
                        startStopIndex = false;
                        objectCount = false;
                        numOfOctetsInRangeSpec = 0;
                        break;
                    case (byte)0x07:
                        startStopIndex = false;
                        objectCount = true;
                        numOfOctetsInRangeSpec = 1;
                        break;
                    case (byte)0x08:
                        startStopIndex = false;
                        objectCount = true;
                        numOfOctetsInRangeSpec = 2;
                        break;
                    case (byte)0x0b:
                        //startStopIndex = false; to do
                        break;
                }

                dNP3Object.IndicesSet = indicesIncluded;
                dNP3Object.ObjectSize = objectCount;
                dNP3Object.StartStopIndex = startStopIndex;
                dNP3Object.NumOfOctetsInRangeSpec = numOfOctetsInRangeSpec;

                if (!indicesIncluded && !startStopIndex && !objectCount)
                {
                    DNP3Objects.Add(dNP3Object);

                    continue;
                }
                else if (!indicesIncluded && startStopIndex)
                {
                    length -= 2;

                    byte[] value = new byte[4];
                    int i = 0;
                    for (i = 0, index++; i < 4; i++, index++)
                    {
                        value[i] = data[index];
                    }

                    int finalValue = BitConverter.ToInt32(value, 0);
                    
                    dNP3Object.Values.Add(finalValue);
                    DNP3Objects.Add(dNP3Object);

                    length -= 4;
                }
            }
        }

        // Create Request
        public void ReadAllAnalogInputPointsRequest(int[] indices)
        {
            RequestHeader requestHeader = new RequestHeader();

            requestHeader.ApplicationControl[7] = true;     // FIR
            requestHeader.ApplicationControl[6] = true;     // FIN
            requestHeader.ApplicationControl[5] = false;    // CON
            requestHeader.ApplicationControl[4] = false;    // UNS

            requestHeader.FunctionCode = ApplicationFunctionCodes.READ;

            Request.RequestHeader = requestHeader;

            ObjectHeader objectHeader = new ObjectHeader()
            {
                Group = 30,
                Variation = 3
            };

            objectHeader.QualifierField[7] = false;     // RES
            objectHeader.QualifierField[6] = false;     // Object prefix code
            objectHeader.QualifierField[5] = false;
            objectHeader.QualifierField[4] = false;

            objectHeader.QualifierField[3] = false;     // Range specifier code
            objectHeader.QualifierField[2] = true;
            objectHeader.QualifierField[1] = true;
            objectHeader.QualifierField[0] = false;

            //int startIndex = indices[0], stopIndex = indices[0];

            //foreach (int index in indices)
            //{
            //    if (index == -1)
            //    {
            //        break;
            //    }

            //    stopIndex = index;
            //}

            //objectHeader.RangeField = new Byte[2];
            //objectHeader.RangeField[0] = BitConverter.GetBytes(startIndex)[0];
            //objectHeader.RangeField[1] = BitConverter.GetBytes(stopIndex)[0];

            Request.ObjectHeaders.Add(objectHeader);

            packRequest = true;
            DNP3TransportFunctionHandler = new DNP3TransportFunctionHandler();
            DNP3TransportFunctionHandler.MakeSegments(PackData(), true);
        }

        public void ReadAllAnalogInputPointsResponse()
        {
            ResponseHeader responseHeader = new ResponseHeader();

            responseHeader.ApplicationControl[7] = true;     // FIR
            responseHeader.ApplicationControl[6] = true;     // FIN
            responseHeader.ApplicationControl[5] = false;    // CON
            responseHeader.ApplicationControl[4] = false;    // UNS

            responseHeader.FunctionCode = ApplicationFunctionCodes.RESPONSE;

            responseHeader.InternalIndications[0] = false;
            responseHeader.InternalIndications[1] = false;
            responseHeader.InternalIndications[2] = false;
            responseHeader.InternalIndications[3] = false;
            responseHeader.InternalIndications[4] = false;
            responseHeader.InternalIndications[5] = false;
            responseHeader.InternalIndications[6] = false;
            responseHeader.InternalIndications[7] = false;
            responseHeader.InternalIndications[8] = false;
            responseHeader.InternalIndications[9] = false;
            responseHeader.InternalIndications[10] = false;
            responseHeader.InternalIndications[11] = false;
            responseHeader.InternalIndications[12] = false;
            responseHeader.InternalIndications[13] = false;
            responseHeader.InternalIndications[14] = false;
            responseHeader.InternalIndications[15] = false;

            Response.ResponseHeader = responseHeader;

            foreach (DNP3Object dnp3Object in DNP3Objects)
            {
                ObjectHeader objectHeader = new ObjectHeader();
                objectHeader.Group = dnp3Object.Group;
                objectHeader.Variation = dnp3Object.Variation;

                objectHeader.QualifierField[7] = false;     // RES
                objectHeader.QualifierField[6] = false;     // Object prefix code
                objectHeader.QualifierField[5] = false;
                objectHeader.QualifierField[4] = false;

                objectHeader.QualifierField[3] = false;     // Range specifier code
                objectHeader.QualifierField[2] = false;
                objectHeader.QualifierField[1] = false;
                objectHeader.QualifierField[0] = false;

                //int startIndex = indices[0], stopIndex = indices[0];

                //foreach (int index in indices)
                //{
                //    if (index == -1)
                //    {
                //        break;
                //    }

                //    stopIndex = index;
                //}

                objectHeader.RangeField = new Byte[2];
                objectHeader.RangeField[0] = 0;
                objectHeader.RangeField[1] = 0;

                Response.ObjectHeaders.Add(objectHeader);

                foreach (int value in dnp3Object.Values)
                {
                    Response.ObjectValues.Add(value);
                }
            }

            packRequest = false;
            DNP3TransportFunctionHandler = new DNP3TransportFunctionHandler();
            DNP3TransportFunctionHandler.MakeSegments(PackData(), false);
        }
    }
}

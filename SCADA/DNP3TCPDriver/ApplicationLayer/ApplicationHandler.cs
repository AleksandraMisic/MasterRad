using DNP3Driver.ApplicationLayer;
using DNP3TCPDriver.TransportFunction;
using DNP3TCPDriver.UserLevel;
using PCCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3TCPDriver.ApplicationLayer
{
    public class ApplicationHandler : IDNP3ApplicationHandler, IProtocolLayer
    {
        public TransportFunctionHandler DNP3TransportFunctionHandler { get; set; }

        public List<DNP3Object> DNP3Objects = new List<DNP3Object>();

        Dictionary<ApplicationFunctionCodes, List<UserLevelObject>> ObjectsUpProcess { get; set; }
        Dictionary<ApplicationFunctionCodes, List<UserLevelObject>> ObjectsDownProcess { get; set; }

        public Request Request { get; set; }
        public Response Response { get; set; }

        private int maxFragmentSize = 2048;

        public ApplicationHandler()
        {
            Request = new Request();
            Response = new Response();
        }
        
        public void ReadAllAnalogInputPointsRequest()
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

            Request.Objects.Add(objectHeader, new List<DNP3Object>());
            
            DNP3TransportFunctionHandler = new TransportFunctionHandler(true);
            DNP3TransportFunctionHandler.PackDown(Request.ToBytes());
        }

        public void ReadAllAnalogInputPointsResponse(byte[] indices)
        {
            ResponseHeader responseHeader = new ResponseHeader();

            responseHeader.ApplicationControl[7] = true;     // FIR // set later
            responseHeader.ApplicationControl[6] = true;     // FIN // set later    
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
            
            DNP3TransportFunctionHandler = new TransportFunctionHandler(false);
            //DNP3TransportFunctionHandler.MakeSegments(PackData(), false);
        }

        public void PackUp(byte[] data)
        {
            int length = data.Length;

            ApplicationFunctionCodes functionCode = (ApplicationFunctionCodes)data[1];
            ObjectsUpProcess = new Dictionary<ApplicationFunctionCodes, List<UserLevelObject>>();
            ObjectsUpProcess.Add(functionCode, new List<UserLevelObject>());

            length -= 2;

            int index = 2;
            while (length > 0)
            {
                length -= 3;

                UserLevelObject userLevelObject = new UserLevelObject();

                ObjectHeader objectHeader = new ObjectHeader();
                objectHeader.Group = data[index++];
                objectHeader.Variation = data[index++];
                objectHeader.QualifierField = new BitArray(new byte[] { data[index++] });

                int valueSize = 0;
                switch (objectHeader.Group)
                {
                    case 30:
                        userLevelObject.PointType = PointType.ANALOG_INPUT;

                        switch (objectHeader.Variation)
                        {
                            case 3:
                                userLevelObject.Variation = Variations.BIT_32_NO_FLAG;
                                valueSize = 4;
                                break;
                        }
                        break;
                }

                byte[] tempQualifier = new byte[1];
                objectHeader.QualifierField.CopyTo(tempQualifier, 0);
                byte qualifierByte = tempQualifier[0];

                byte[]  rangeField = null;
                int count = 0;

                switch (qualifierByte)
                {
                    case 0x00:
                        userLevelObject.IndicesPresent = false;
                        userLevelObject.RangeFieldPresent = true;
                        userLevelObject.RangePresent = true;
                        userLevelObject.ObjectCountPresent = false;

                        count = 2;
                        for (int i = 0; i < count; i++)
                        {
                            objectHeader.RangeField[i] = data[index++];
                        }

                        length -= count;

                        userLevelObject.StartIndex = objectHeader.RangeField[0];
                        userLevelObject.StopIndex = objectHeader.RangeField[1];

                        if (functionCode == ApplicationFunctionCodes.RESPONSE ||
                            functionCode == ApplicationFunctionCodes.WRITE)
                        {
                            byte[] value = new byte[valueSize];
                            for (int i = userLevelObject.StartIndex; i < userLevelObject.StopIndex; i++)
                            {
                                for (int j = 0; j < valueSize; j++)
                                {
                                    value[j] = data[index++];
                                    --length;
                                }
                            }

                            userLevelObject.Values.Add(value);
                        }

                        break;

                    case 0x01:
                        userLevelObject.IndicesPresent = false;
                        userLevelObject.RangeFieldPresent = true;
                        userLevelObject.RangePresent = true;
                        userLevelObject.ObjectCountPresent = false;

                        count = 4;
                        for (int i = 0; i < count; i++)
                        {
                            objectHeader.RangeField[i] = data[index++];
                        }

                        length -= count;

                        byte[] tempStart = new byte[2];
                        byte[] tempStop = new byte[2];
                        for (int i = 0; i < 2; i++)
                        {
                            tempStart[i] = objectHeader.RangeField[i];
                        }
                        for (int i = 0; i < 2; i++)
                        {
                            tempStop[i] = objectHeader.RangeField[i+2];
                        }

                        short[] start = new short[1];
                        short[] stop = new short[1];
                        tempStart.CopyTo(start, 0);
                        tempStop.CopyTo(stop, 0);

                        userLevelObject.StartIndex = start[0];
                        userLevelObject.StopIndex = stop[0];

                        if (functionCode == ApplicationFunctionCodes.RESPONSE ||
                            functionCode == ApplicationFunctionCodes.WRITE)
                        {
                            byte[] value = new byte[valueSize];
                            for (int i = userLevelObject.StartIndex; i < userLevelObject.StopIndex; i++)
                            {
                                for (int j = 0; j < valueSize; j++)
                                {
                                    value[j] = data[index++];
                                    --length;
                                }
                            }

                            userLevelObject.Values.Add(value);
                        }

                        break;

                    case 0x06:
                        userLevelObject.IndicesPresent = false;
                        userLevelObject.RangeFieldPresent = false;
                        userLevelObject.RangePresent = false;
                        userLevelObject.ObjectCountPresent = false;
                        break;

                    case 0x07:
                        userLevelObject.IndicesPresent = false;
                        userLevelObject.ObjectSizePresent = false;
                        userLevelObject.RangeFieldPresent = true;
                        userLevelObject.RangePresent = false;
                        userLevelObject.ObjectCountPresent = true;

                        count = 1;
                        for (int i = 0; i < count; i++)
                        {
                            objectHeader.RangeField[i] = data[index++];
                        }

                        length -= count;

                        userLevelObject.ObjectCount = objectHeader.RangeField[0];

                        if (functionCode == ApplicationFunctionCodes.RESPONSE ||
                            functionCode == ApplicationFunctionCodes.WRITE)
                        {
                            byte[] value = new byte[valueSize];
                            for (int i = 0; i < userLevelObject.ObjectCount; i++)
                            {
                                for (int j = 0; j < valueSize; j++)
                                {
                                    value[j] = data[index++];
                                    --length;
                                }
                            }

                            userLevelObject.Values.Add(value);
                        }

                        break;

                    case 0x08:
                        userLevelObject.IndicesPresent = false;
                        userLevelObject.ObjectSizePresent = false;
                        userLevelObject.RangeFieldPresent = true;
                        userLevelObject.RangePresent = false;
                        userLevelObject.ObjectCountPresent = true;

                        count = 2;
                        for (int i = 0; i < count; i++)
                        {
                            objectHeader.RangeField[i] = data[index++];
                        }

                        length -= count;

                        short[] tempCount = new short[1];
                        objectHeader.RangeField.CopyTo(tempCount, 0);

                        userLevelObject.ObjectCount = tempCount[0];

                        if (functionCode == ApplicationFunctionCodes.RESPONSE ||
                            functionCode == ApplicationFunctionCodes.WRITE)
                        {
                            byte[] value = new byte[valueSize];
                            for (int i = 0; i < userLevelObject.ObjectCount; i++)
                            {
                                for (int j = 0; j < valueSize; j++)
                                {
                                    value[j] = data[index++];
                                    --length;
                                }
                            }

                            userLevelObject.Values.Add(value);
                        }

                        break;

                    case 0x17:
                        userLevelObject.IndicesPresent = true;
                        userLevelObject.ObjectSizePresent = false;
                        userLevelObject.RangeFieldPresent = true;
                        userLevelObject.RangePresent = false;
                        userLevelObject.ObjectCountPresent = true;

                        count = 1;
                        for (int i = 0; i < count; i++)
                        {
                            objectHeader.RangeField[i] = data[index++];
                        }

                        length -= count;

                        userLevelObject.ObjectCount = objectHeader.RangeField[0];
                        
                        if (functionCode == ApplicationFunctionCodes.RESPONSE ||
                            functionCode == ApplicationFunctionCodes.WRITE)
                        {
                            byte[] value = new byte[valueSize];
                            for (int i = 0; i < userLevelObject.ObjectCount; i++)
                            {
                                byte[] tempIndex = new byte[1];
                                tempIndex[0] = data[index++];
                                userLevelObject.Indices.Add(BitConverter.ToInt32(tempIndex, 0));
                                length -= 1;
                                for (int j = 0; j < valueSize; j++)
                                {
                                    value[j] = data[index++];
                                    --length;
                                }
                            }

                            userLevelObject.Values.Add(value);
                        }
                        else
                        {
                            for (int i = 0; i < userLevelObject.ObjectCount; i++)
                            {
                                byte[] tempIndex = new byte[1];
                                tempIndex[0] = data[index++];
                                userLevelObject.Indices.Add(BitConverter.ToInt32(tempIndex, 0));
                                length -= 1;
                            }
                        }

                        break;

                    case 0x28:
                        userLevelObject.IndicesPresent = true;
                        userLevelObject.ObjectSizePresent = false;
                        userLevelObject.RangeFieldPresent = true;
                        userLevelObject.RangePresent = false;
                        userLevelObject.ObjectCountPresent = true;

                        count = 2;
                        for (int i = 0; i < count; i++)
                        {
                            objectHeader.RangeField[i] = data[index++];
                        }

                        length -= count;

                        tempCount = new short[1];
                        objectHeader.RangeField.CopyTo(tempCount, 0);

                        userLevelObject.ObjectCount = tempCount[0];
                        
                        if (functionCode == ApplicationFunctionCodes.RESPONSE ||
                            functionCode == ApplicationFunctionCodes.WRITE)
                        {
                            byte[] value = new byte[valueSize];
                            for (int i = 0; i < userLevelObject.ObjectCount; i++)
                            {
                                byte[] tempIndex = new byte[2];
                                tempIndex[0] = data[index++];
                                tempIndex[1] = data[index++];
                                userLevelObject.Indices.Add(BitConverter.ToInt32(tempIndex, 0));
                                length -= 1;
                                for (int j = 0; j < valueSize; j++)
                                {
                                    value[j] = data[index++];
                                    --length;
                                }
                            }

                            userLevelObject.Values.Add(value);
                        }
                        else
                        {
                            byte[] tempIndex = new byte[2];
                            tempIndex[0] = data[index++];
                            tempIndex[1] = data[index++];
                            userLevelObject.Indices.Add(BitConverter.ToInt32(tempIndex, 0));
                            length -= 2;
                        }

                        break;

                    case 0x5b:
                        userLevelObject.IndicesPresent = false;
                        userLevelObject.ObjectSizePresent = true;
                        userLevelObject.RangeFieldPresent = false;
                        userLevelObject.RangePresent = false;
                        userLevelObject.ObjectCountPresent = false;

                        count = 1;
                        for (int i = 0; i < count; i++)
                        {
                            objectHeader.RangeField[i] = data[index++];
                        }

                        break;
                }

                ObjectsUpProcess[functionCode].Add(userLevelObject);
            }
        }

        public void PackDown(byte[] data)
        {
            byte[] tempFragment = new byte[maxFragmentSize];
            int index = 0;

            Header header = null;

            if (ObjectsDownProcess.Keys.ElementAt(0) == ApplicationFunctionCodes.RESPONSE)
            {
                header = new ResponseHeader();

                ((ResponseHeader)header).InternalIndications[0] = false;
                ((ResponseHeader)header).InternalIndications[1] = false;
                ((ResponseHeader)header).InternalIndications[2] = false;
                ((ResponseHeader)header).InternalIndications[3] = false;
                ((ResponseHeader)header).InternalIndications[4] = false;
                ((ResponseHeader)header).InternalIndications[5] = false;
                ((ResponseHeader)header).InternalIndications[6] = false;
                ((ResponseHeader)header).InternalIndications[7] = false;
                ((ResponseHeader)header).InternalIndications[8] = false;
                ((ResponseHeader)header).InternalIndications[9] = false;
                ((ResponseHeader)header).InternalIndications[10] = false;
                ((ResponseHeader)header).InternalIndications[11] = false;
                ((ResponseHeader)header).InternalIndications[12] = false;
                ((ResponseHeader)header).InternalIndications[13] = false;
                ((ResponseHeader)header).InternalIndications[14] = false;
                ((ResponseHeader)header).InternalIndications[15] = false;
            }
            else
            {
                header = new RequestHeader();
            }

            header.ApplicationControl[7] = true;     // FIR
            header.ApplicationControl[6] = true;     // FIN
            header.ApplicationControl[5] = false;    // CON
            header.ApplicationControl[4] = false;    // UNS

            byte[] tempHeader = header.ToBytes();
            tempHeader.CopyTo(tempFragment, index);
            index += tempHeader.Count();

            ApplicationFunctionCodes functionCode =  header.FunctionCode = ObjectsDownProcess.Keys.ElementAt(0);

            foreach (UserLevelObject userLevelObject in ObjectsDownProcess[0])
            {
                ObjectHeader objectHeader = new ObjectHeader();

                switch (userLevelObject.PointType)
                {
                    case PointType.ANALOG_INPUT:
                        objectHeader.Group = 30;

                        switch (userLevelObject.Variation)
                        {
                            case Variations.BIT_32_NO_FLAG:
                                objectHeader.Variation = 3;
                                break;
                        }
                        break;
                }

                objectHeader.QualifierField[7] = false;
                if (userLevelObject.IndicesPresent)
                {
                    objectHeader.QualifierField[6] = false;
                    objectHeader.QualifierField[5] = false;
                    objectHeader.QualifierField[4] = true;
                }
                else if(userLevelObject.ObjectSizePresent)
                {
                    // TO DO
                }
                else
                {
                    objectHeader.QualifierField[6] = false;
                    objectHeader.QualifierField[5] = false;
                    objectHeader.QualifierField[4] = false;
                }

                if (userLevelObject.RangeFieldPresent)
                {
                    if (userLevelObject.RangePresent)
                    {
                        objectHeader.RangeField = new byte[2];
                        objectHeader.RangeField[0] = (byte)userLevelObject.StartIndex;
                        objectHeader.RangeField[1] = (byte)userLevelObject.StopIndex;
                    }
                    else if (userLevelObject.ObjectCountPresent)
                    {
                        objectHeader.RangeField = new byte[2];
                        byte[] temp = BitConverter.GetBytes(userLevelObject.ObjectCount);
                        objectHeader.RangeField[0] = temp[0];
                        objectHeader.RangeField[1] = temp[1];
                    }
                }

                byte[] tempObjHeader = objectHeader.ToBytes();
                tempObjHeader.CopyTo(tempFragment, index);
                index += tempObjHeader.Count();

                byte[] tempQualifier = new byte[1];
                objectHeader.QualifierField.CopyTo(tempQualifier, 0);
                byte qualifierByte = tempQualifier[0];

                switch (qualifierByte)
                {
                    case 0x00:

                        if (functionCode == ApplicationFunctionCodes.RESPONSE ||
                            functionCode == ApplicationFunctionCodes.WRITE)
                        {
                            foreach (byte[] value in userLevelObject.Values)
                            {
                                value.CopyTo(tempFragment, index);
                                index += value.Count();
                            }
                        }

                        break;

                    case 0x01:
                        if (functionCode == ApplicationFunctionCodes.RESPONSE ||
                            functionCode == ApplicationFunctionCodes.WRITE)
                        {
                            foreach (byte[] value in userLevelObject.Values)
                            {
                                value.CopyTo(tempFragment, index);
                                index += value.Count();
                            }
                        }

                        break;

                    case 0x06:
                        break;

                    case 0x07:
                        if (functionCode == ApplicationFunctionCodes.RESPONSE ||
                            functionCode == ApplicationFunctionCodes.WRITE)
                        {
                            foreach (byte[] value in userLevelObject.Values)
                            {
                                value.CopyTo(tempFragment, index);
                                index += value.Count();
                            }
                        }

                        break;

                    case 0x08:
                        if (functionCode == ApplicationFunctionCodes.RESPONSE ||
                            functionCode == ApplicationFunctionCodes.WRITE)
                        {
                            foreach (byte[] value in userLevelObject.Values)
                            {
                                value.CopyTo(tempFragment, index);
                                index += value.Count();
                            }
                        }

                        break;

                    case 0x17:
                        if (functionCode == ApplicationFunctionCodes.RESPONSE ||
                            functionCode == ApplicationFunctionCodes.WRITE)
                        {
                            for (int i = 0; i < userLevelObject.Values.Count(); i++)
                            {
                                tempFragment[index++] = (byte)userLevelObject.Indices[i];
                                byte[] tempValue = userLevelObject.Values[i];
                                tempValue.CopyTo(tempFragment, index);
                                index += tempValue.Count();
                            }
                        }
                        else
                        {
                            for (int i = 0; i < userLevelObject.Indices.Count(); i++)
                            {
                                tempFragment[index++] = (byte)userLevelObject.Indices[i];
                            }
                        }

                        break;

                    case 0x28:
                        if (functionCode == ApplicationFunctionCodes.RESPONSE ||
                            functionCode == ApplicationFunctionCodes.WRITE)
                        {
                            for (int i = 0; i < userLevelObject.Values.Count(); i++)
                            {
                                byte[] tempIndices = BitConverter.GetBytes(userLevelObject.Indices[i]);
                                tempFragment[index++] = tempIndices[0];
                                tempFragment[index++] = tempIndices[1];
                                byte[] tempValue = userLevelObject.Values[i];
                                tempValue.CopyTo(tempFragment, index);
                                index += tempValue.Count();
                            }
                        }
                        else
                        {
                            for (int i = 0; i < userLevelObject.Indices.Count(); i++)
                            {
                                byte[] tempIndices = BitConverter.GetBytes(userLevelObject.Indices[i]);
                                tempFragment[index++] = tempIndices[0];
                                tempFragment[index++] = tempIndices[1];
                            }
                        }

                        break;

                    case 0x5b:

                        break;
                }

                byte[] finalFragment = new byte[index];
                for (int i = 0; i < index; i++)
                {
                    finalFragment[i] = tempFragment[i];
                }

                DNP3TransportFunctionHandler = new TransportFunctionHandler(true);
                DNP3TransportFunctionHandler.PackDown(finalFragment);
            }
        }
    }
}

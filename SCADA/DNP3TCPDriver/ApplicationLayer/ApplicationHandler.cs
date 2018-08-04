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
    public class ApplicationHandler
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

                byte[] rangeField = null;
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
                            tempStop[i] = objectHeader.RangeField[i + 2];
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

        public List<byte[]> PackDown(List<UserLevelObject> userLevelObjects, ApplicationFunctionCodes functionCode, bool isRequest, bool isMaster)
        {
            if (userLevelObjects.Count == 0)
            {
                return null;
            }

            List<byte[]> fragments = new List<byte[]>();
            byte[] tempFragment = new byte[maxFragmentSize];
            byte[] finalFragment = null;
            int index = 0;
            bool fragmentInProgress = true;

            Header header = null;

            if (functionCode == ApplicationFunctionCodes.RESPONSE)
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

            header.ApplicationControl[7] = false;    // FIR
            header.ApplicationControl[6] = false;    // FIN
            header.ApplicationControl[5] = false;    // CON
            header.ApplicationControl[4] = false;    // UNS

            header.FunctionCode = functionCode;

            byte[] tempHeader = header.ToBytes();
            tempHeader.CopyTo(tempFragment, index);
            index += tempHeader.Count();

            byte[] tempObjHeader = null;

            foreach (UserLevelObject userLevelObject in userLevelObjects)
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
                else if (userLevelObject.ObjectSizePresent)
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

                tempObjHeader = objectHeader.ToBytes();

                if ((index += tempObjHeader.Count()) > maxFragmentSize)
                {
                    finalFragment = new byte[index];
                    for (int i = 0; i < index; i++)
                    {
                        finalFragment[i] = tempFragment[i];
                    }

                    fragments.Add(finalFragment);
                    
                    index = objectHeader.ToBytes().Count();
                }

                int startIndex, stopIndex;
                startIndex = userLevelObject.StartIndex;
                stopIndex = userLevelObject.StopIndex;

                byte[] tempQualifier = new byte[1];
                objectHeader.QualifierField.CopyTo(tempQualifier, 0);
                byte qualifierByte = tempQualifier[0];

                switch (qualifierByte)
                {
                    case 0x00:
                    case 0x01:

                        int counter = -1;
                        foreach (byte[] value in userLevelObject.Values)
                        {
                            counter++;
                            if ((index + value.Count()) <= maxFragmentSize)
                            {
                                value.CopyTo(tempFragment, index);
                                index += value.Count();
                                stopIndex = userLevelObject.Indices[counter];

                                fragmentInProgress = true;
                            }
                            else
                            {
                                objectHeader.RangeField[0] = (byte)startIndex;
                                objectHeader.RangeField[1] = (byte)stopIndex;

                                tempObjHeader = objectHeader.ToBytes();
                                tempObjHeader.CopyTo(tempFragment, tempHeader.Count());

                                finalFragment = new byte[index];
                                for (int i = 0; i < index; i++)
                                {
                                    finalFragment[i] = tempFragment[i];
                                }

                                fragments.Add(finalFragment);

                                tempHeader = header.ToBytes();
                                tempHeader.CopyTo(tempFragment, index);
                                index = tempHeader.Count();

                                index += objectHeader.ToBytes().Count();

                                value.CopyTo(tempFragment, index);
                                index += value.Count();
                                startIndex = stopIndex = userLevelObject.Indices[counter];

                                fragmentInProgress = false;
                            }
                        }

                        if (fragmentInProgress)
                        {
                            objectHeader.RangeField[0] = (byte)startIndex;
                            objectHeader.RangeField[1] = (byte)stopIndex;

                            tempObjHeader = objectHeader.ToBytes();
                            tempObjHeader.CopyTo(tempFragment, tempHeader.Count());
                        }

                        break;

                    case 0x07:
                    case 0x08:
                        break;

                    case 0x06:
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
            }

            finalFragment = new byte[index];
            for (int i = 0; i < index; i++)
            {
                finalFragment[i] = tempFragment[i];
            }

            fragments.Add(finalFragment);
            byte[] tempControl = new byte[1];

            if (fragments.Count() == 1)
            {
                BitArray appControl = new BitArray(new byte[1] { fragments.First()[0] });
                appControl[7] = true;
                appControl[6] = true;
                appControl.CopyTo(tempControl, 0);
                fragments.First()[0] = tempControl[0];
            }
            else
            {
                BitArray appControl = new BitArray(new byte[1] { fragments.First()[0] });
                appControl[7] = true;
                appControl.CopyTo(tempControl, 0);
                fragments.First()[0] = tempControl[0];

                appControl = new BitArray(new byte[1] { fragments.Last()[0] });
                appControl[6] = true;
                fragments.Last()[0] = tempControl[0];
            }

            List<byte[]> returnValue = new List<byte[]>();
            DNP3TransportFunctionHandler = new TransportFunctionHandler();

            foreach (byte[] fragment in fragments)
            {
                List<byte[]> segments = DNP3TransportFunctionHandler.PackDown(fragment, isRequest, isMaster);

                foreach (byte[] segment in segments)
                {
                    returnValue.Add(segment);
                }
            }

            return returnValue;
        }
    }
}

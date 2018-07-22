using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3TCPDriver.ApplicationLayer
{
    public class Response
    {
        public ResponseHeader ResponseHeader { get; set; }
        public List<ObjectHeader> ObjectHeaders { get; set; }
        public List<int> ObjectValues { get; set; }

        public Response()
        {
            ResponseHeader = new ResponseHeader();
            ObjectHeaders = new List<ObjectHeader>();
            ObjectValues = new List<int>();
        }
        public Byte[] GetResponseInBytes()
        {
            int totalSize = 4;

            foreach (ObjectHeader objectHeader in ObjectHeaders)
            {
                totalSize += 3 + objectHeader.RangeField.Count();
            }

            totalSize += ObjectValues.Count() * 4;

            Byte[] array = new Byte[totalSize];
            ResponseHeader.ApplicationControl.CopyTo(array, 0);
            array[1] = (Byte)ResponseHeader.FunctionCode;
            ResponseHeader.InternalIndications.CopyTo(array, 2);

            int i = 4;
            foreach (ObjectHeader objectHeader in ObjectHeaders)
            {
                array[i++] = objectHeader.Group;
                array[i++] = objectHeader.Variation;
                objectHeader.QualifierField.CopyTo(array, i++);

                if (objectHeader.RangeField.Count() == 1)
                {
                    array[i++] = objectHeader.RangeField[0];
                }
                else if (objectHeader.RangeField.Count() == 2)
                {
                    array[i++] = objectHeader.RangeField[0];
                    array[i++] = objectHeader.RangeField[1];
                }
                else if (objectHeader.RangeField.Count() == 4)
                {
                    array[i++] = objectHeader.RangeField[0];
                    array[i++] = objectHeader.RangeField[1];
                    array[i++] = objectHeader.RangeField[2];
                    array[i++] = objectHeader.RangeField[3];
                }
            }

            foreach (int value in ObjectValues)
            {
                byte[] temp = BitConverter.GetBytes(value);
                temp.CopyTo(array, i);
                i += 4;
            }

            return array;
        }
    }
}

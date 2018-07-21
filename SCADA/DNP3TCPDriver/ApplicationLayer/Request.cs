using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3TCPDriver.ApplicationLayer
{
    public class Request
    {
        public RequestHeader RequestHeader { get; set; }
        public List<ObjectHeader> ObjectHeaders { get; set; }

        public Request()
        {
            RequestHeader = new RequestHeader();
            ObjectHeaders = new List<ObjectHeader>();
        }

        public Byte[] GetRequestInBytes()
        {
            int totalSize = 2 + 3;

            foreach (ObjectHeader objectHeader in ObjectHeaders)
            {
                totalSize += objectHeader.RangeField.Count();
            }

            Byte[] array = new Byte[totalSize];
            RequestHeader.ApplicationControl.CopyTo(array, 0);
            array[1] = (Byte)RequestHeader.FunctionCode;

            int i = 2;
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

            return array;
        }
    }
}

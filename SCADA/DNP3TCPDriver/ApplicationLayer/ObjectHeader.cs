using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3TCPDriver.ApplicationLayer
{
    public class ObjectHeader : IByteable
    {
        public Byte Group { get; set; }
        public Byte Variation { get; set; }
        public BitArray QualifierField { get; set; }
        public Byte[] RangeField { get; set; }

        public ObjectHeader()
        {
            QualifierField = new BitArray(8);
        }

        public byte[] ToBytes()
        {
            int maxLength = 3 + 8;
            byte[] temp = new byte[maxLength];

            int index = 0;
            temp[index++] = Group;
            temp[index++] = Variation;

            byte[] tempQualifier = new byte[1];
            QualifierField.CopyTo(tempQualifier, 0);

            temp[index++] = tempQualifier[0];

            if (RangeField != null)
            {
                for (int i = 0; i < RangeField.Count(); i++)
                {
                    temp[index++] = RangeField[i];
                }
            }

            byte[] final = new byte[index];
            for (int i = 0; i < index; i++)
            {
                final[i] = temp[i];
            }

            return final;
        }
    }
}

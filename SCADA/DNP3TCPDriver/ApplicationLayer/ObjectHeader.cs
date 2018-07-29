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
            throw new NotImplementedException();
        }
    }
}

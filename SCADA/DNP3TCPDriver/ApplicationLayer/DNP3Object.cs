using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3Driver.ApplicationLayer
{
    public class DNP3Object
    {
        public byte Group { get; set; }
        public byte Variation { get; set; }

        public bool IndicesSet { get; set; }
        public bool ObjectSize { get; set; }
        public bool StartStopIndex { get; set; }
        public bool RangeQualifierSize { get; set; }
        public int NumOfOctetsInRangeSpec { get; set; }
        public List<int> Values { get; set; }

        public DNP3Object()
        {
            Values = new List<int>();
        }
    }
}

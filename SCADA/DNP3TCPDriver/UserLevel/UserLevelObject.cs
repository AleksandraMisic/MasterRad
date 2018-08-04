using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3TCPDriver.UserLevel
{
    public class UserLevelObject
    {
        public Variations Variation { get; set; }
        public PointType PointType { get; set; }

        public List<int> Indices { get; set; }
        public List<byte[]> Values { get; set; }

        public int StartIndex { get; set; }
        public int StopIndex { get; set; }

        public int ObjectCount { get; set; }

        public bool RangeFieldPresent { get; set; }
        public bool IndicesPresent { get; set; }
        public bool ObjectSizePresent { get; set; }
        public bool RangePresent { get; set; }
        public bool ObjectCountPresent { get; set; }

        public UserLevelObject()
        {
            Values = new List<byte[]>();
            Indices = new List<int>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3TCPDriver.UserLevel.Objects
{
    public class StaticObject : UserLevelObject
    {
        public StaticObjectVariations Variation { get; set; }
        public PointType PointType { get; set; }

        public byte[] Indices { get; set; }
        public object[] Values { get; set; }
    }
}

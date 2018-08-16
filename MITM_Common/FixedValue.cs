using DNP3TCPDriver.UserLevel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITM_Common
{
    public class FixedValue
    {
        public PointType pointType { get; set; }

        public int Index { get; set; }

        public int Value { get; set; }
    }
}

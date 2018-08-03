using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3TCPDriver.UserLevel
{
    public interface IDNP3UserLayer
    {
        List<byte[]> ReadAllAnalogInputPointsRequest(string rtuName);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3TCPDriver.ApplicationLayer
{
    public interface IDNP3ApplicationHandler
    {
        void ReadAllAnalogInputPointsRequest(int[] indices);

        void ReadAllAnalogInputPointsResponse();
    }
}

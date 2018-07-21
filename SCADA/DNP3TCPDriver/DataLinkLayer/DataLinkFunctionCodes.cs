using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3TCPDriver.DataLinkLayer
{
    public enum DataLinkFunctionCodes : Byte
    {
        RESET_LINK_STATES = 0,
        TEST_LINK_STATES = 1,
        CONFIRMED_USER_DATA = 3,
        UNCONFIRMED_USER_DATA = 4,
        REQUEST_LINK_STATUS = 9
    }
}

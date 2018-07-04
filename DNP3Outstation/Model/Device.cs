using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3Outstation.Model
{
    public class Device
    {
        public string DeviceFunction { get; set; }
        public string VendorName { get; set; }
        public string DeviceName { get; set; }
        public string HardwareVersion { get; set; }
        public string SoftwareVersion { get; set; }

    }
}

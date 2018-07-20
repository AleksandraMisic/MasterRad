using DNP3ConfigParser.Configurations;
using DNP3ConfigParser.Configurations.DNP3DeviceProfileJan2010ConfigModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3ConfigParser.Configuration.DNP3DeviceProfileJan2010ConfigModel
{
    public class CompleteConfiguration : UniversalConfiguration
    {
        public DeviceConfiguration DeviceConfiguration { get; set; }
        public NetworkConfiguration NetworkConfiguration { get; set; }

        public CompleteConfiguration()
        {
            DeviceConfiguration = new DeviceConfiguration();
            NetworkConfiguration = new NetworkConfiguration();
        }
    }
}

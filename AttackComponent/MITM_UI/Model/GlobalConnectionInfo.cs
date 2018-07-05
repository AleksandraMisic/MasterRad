using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITM_UI.Model
{
    public class GlobalConnectionInfo
    {
        public ConnectionState ConnectionState { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }

        public string FriendlyName { get; set; }

        public byte[] IPAddress { get; set; }

        public byte[] MACAddress { get; set; }

        public byte[] DefaultGateway { get; set; }

        public byte[] SubnetMask { get; set; }

        public bool RoutingEnabled { get; set; }
    }
}

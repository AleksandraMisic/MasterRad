using DNP3ConfigParser.Configuration;
using DNP3ConfigParser.Enums.NetworkConfig;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3ConfigParser.Configurations.DNP3DeviceProfileJan2010ConfigModel
{
    public class NetworkConfiguration : UniversalNetworkConfiguration, INotifyPropertyChanged
    {
        private string portName;
        private string ipAddress;

        private string remoteIPAddress;

        private TypeOfEndpoint typeOfEndpointCurrentValue;

        public string PortName
        {
            get
            {
                return "Port name:\n\t" + portName;
            }
            set
            {
                portName = value;
                RaisePropertyChanged("PortName");
            }
        }

        public string IpAddress
        {
            get
            {
                return ipAddress;
            }
            set
            {
                ipAddress = value;
                RaisePropertyChanged("IpAddressToString");
            }
        }

        public string IpAddressToString
        {
            get
            {
                return "IP address:\n\t" + ipAddress;
            }
        }

        public TypeOfEndpoint TypeOfEndpointCurrentValue
        {
            get
            {
                return typeOfEndpointCurrentValue;
            }
            set
            {
                typeOfEndpointCurrentValue = value;
                RaisePropertyChanged("TypeOfEndpointCurrentValueToString");
            }
        }

        public List<TypeOfEndpoint> TypeOfEndpointCapabilities { get; set; }

        public List<TCPListenPortNumber> TCPListenPortCapabilities { get; set; }

        public List<TCPListenPortNumber> TCPListenPortRemoteCapabilities { get; set; }

        public string RemoteIPAddress
        {
            get
            {
                return remoteIPAddress;
            }
            set
            {
                remoteIPAddress = value;
                RaisePropertyChanged("RemoteIPAddress");
            }
        }

        public string TypeOfEndpointCurrentValueToString
        {
            get
            {
                return "Type of endpoint:\n\t" + TypeOfEndpointCurrentValue.ToString();
            }
        }

        public NetworkConfiguration()
        {
            TypeOfEndpointCapabilities = new List<TypeOfEndpoint>();
            TCPListenPortCapabilities = new List<TCPListenPortNumber>();
            TCPListenPortRemoteCapabilities = new List<TCPListenPortNumber>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}

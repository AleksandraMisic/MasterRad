using DNP3ConfigParser.Enums.DeviceConfig;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3ConfigParser.Configurations.DNP3DeviceProfileJan2010ConfigModel
{
    public class DeviceConfiguration : INotifyPropertyChanged
    {
        private DeviceFunction deviceFunctionCurrentValue;
        private string deviceName;

        public List<DeviceFunction> DeviceFunctionCapabilities { get; set; }

        public DeviceFunction DeviceFunctionCurrentValue
        {
            get
            {
                return deviceFunctionCurrentValue;
            }
            set
            {
                deviceFunctionCurrentValue = value;
                RaisePropertyChanged("DeviceFunctionCurrentValueToString");
            }
        }

        public string DeviceFunctionCurrentValueToString
        {
            get
            {
                return "Function:\n\t" + DeviceFunctionCurrentValue.ToString();
            }
        }

        public string DeviceName
        {
            get
            {
                return deviceName;
            }
            set
            {
                deviceName = value;
                RaisePropertyChanged("DeviceNameToString");
            }
        }

        public string DeviceNameToString
        {
            get
            {
                return "Device name:\n\t" + deviceName;
            }
        }

        public DeviceConfiguration()
        {
            DeviceFunctionCapabilities = new List<DeviceFunction>();
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

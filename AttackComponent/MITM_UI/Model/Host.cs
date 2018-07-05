using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITM_UI.Model
{
    public class Host :  INotifyPropertyChanged
    {
        private string ipAddressString;
        private string macAddressString;

        private byte[] ipAddressArray;
        private byte[] macAddressArray;

        public string IPAddressString
        {
            get
            {
                return ipAddressString;
            }
            set
            {
                ipAddressString = value;
                RaisePropertyChanged("IPAddressString");
            }
        }

        public string MACAddressString
        {
            get
            {
                return macAddressString;
            }
            set
            {
                macAddressString = value;
                RaisePropertyChanged("MACAddressString");
            }
        }

        public byte[] IPAddressArray
        {
            get
            {
                return ipAddressArray;
            }
            set
            {
                ipAddressArray = value;
                RaisePropertyChanged("IPAddressArray");
            }
        }

        public byte[] MACAddressArray
        {
            get
            {
                return macAddressArray;
            }
            set
            {
                macAddressArray = value;
                RaisePropertyChanged("MACAddressArray");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}

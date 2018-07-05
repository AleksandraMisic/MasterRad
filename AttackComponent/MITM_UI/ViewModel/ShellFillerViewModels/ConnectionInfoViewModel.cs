using MITM_UI.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UIShell.ViewModel;

namespace MITM_UI.ViewModel.ShellFillerViewModels
{
    public class ConnectionInfoViewModel : SingleShellFillerViewModel, INotifyPropertyChanged
    {
        private GlobalConnectionInfo globalConnectionInfo;

        private static bool isOpen;

        private static bool isConnected;
        private string description;
        private string friendlyName;
        private string iPAddress;
        private string mACAddress;
        private string subnetMask;

        public ConnectionInfoViewModel(GlobalConnectionInfo globalConnectionInfo)
        {
            this.globalConnectionInfo = globalConnectionInfo;
        }

        #region Properties
        public override bool IsOpen
        {
            get { return isOpen; }
            set { isOpen = value; }
        }

        public bool IsConnected
        {
            get
            {
                return isConnected;
            }
            set
            {
                isConnected = value;
                RaisePropertyChanged("IsConnected");
            }
        }

        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
                RaisePropertyChanged("Description");
            }
        }

        public string FriendlyName
        {
            get
            {
                return friendlyName;
            }
            set
            {
                friendlyName = value;
                RaisePropertyChanged("FriendlyName");
            }
        }

        public string IPAddress
        {
            get
            {
                return iPAddress;
            }
            set
            {
                iPAddress = value;
                RaisePropertyChanged("IPAddress");
            }
        }

        public string MACAddress
        {
            get
            {
                return mACAddress;
            }
            set
            {
                mACAddress = value;
                RaisePropertyChanged("MACAddress");
            }
        }

        public string SubnetMask
        {
            get
            {
                return subnetMask;
            }
            set
            {
                subnetMask = value;
                RaisePropertyChanged("SubnetMask");
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public void GetNetworkInfo()
        {
            while (true)
            {
                if (globalConnectionInfo.ConnectionState == ConnectionState.DISCONNECTED)
                {
                    this.IsConnected = false;
                    continue;
                }

                this.IsConnected = true;

                this.friendlyName = globalConnectionInfo.FriendlyName;
                this.Description = globalConnectionInfo.Description;

                this.IPAddress = string.Empty;
                for (int i = 0; i < 4; i++)
                {
                    this.IPAddress += globalConnectionInfo.IPAddress[i].ToString();

                    if (i != 3)
                    {
                        this.IPAddress += ".";
                    }
                }

                this.SubnetMask = string.Empty;
                for (int i = 0; i < 4; i++)
                {
                    this.SubnetMask += globalConnectionInfo.SubnetMask[i].ToString();

                    if (i != 3)
                    {
                        this.SubnetMask += ".";
                    }
                }

                this.MACAddress = string.Empty;
                for (int i = 0; i < 6; i++)
                {
                    this.MACAddress += globalConnectionInfo.MACAddress[i].ToString("X");

                    if (i != 5)
                    {
                        this.MACAddress += " : ";
                    }
                }

                Thread.Sleep(500);
            }
        }
    }
}

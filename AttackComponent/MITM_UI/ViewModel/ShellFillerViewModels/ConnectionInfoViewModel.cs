using MITM_Common.MITM_Service;
using MITM_UI.Model;
using MITM_UI.Model.GlobalInfo;
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
    public class ConnectionInfoViewModel : SingleShellFillerViewModel
    {
        private static bool isOpen;

        private static bool isConnected;
        private string description;
        private string friendlyName;
        private string iPAddress;
        private string mACAddress;
        private string subnetMask;

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

        public void CheckForConnectionChange()
        {
            GlobalConnectionInfo connectionInfo = Database.GlobalConnectionInfo;

            if (connectionInfo.ConnectionState == ConnectionState.DISCONNECTED)
            {
                this.IsConnected = false;
                return;
            }

            this.IsConnected = true;

            this.friendlyName = connectionInfo.FriendlyName;
            this.Description = connectionInfo.Description;

            this.IPAddress = string.Empty;
            for (int i = 0; i < 4; i++)
            {
                this.IPAddress += connectionInfo.IPAddress[i].ToString();

                if (i != 3)
                {
                    this.IPAddress += ".";
                }
            }

            this.SubnetMask = string.Empty;
            for (int i = 0; i < 4; i++)
            {
                this.SubnetMask += connectionInfo.SubnetMask[i].ToString();

                if (i != 3)
                {
                    this.SubnetMask += ".";
                }
            }

            this.MACAddress = string.Empty;
            for (int i = 0; i < 6; i++)
            {
                this.MACAddress += connectionInfo.MACAddress[i].ToString("X");

                if (i != 5)
                {
                    this.MACAddress += " : ";
                }
            }
        }
    }
}

using MITM_UI.Model;
using MITM_UI.Model.GlobalInfo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using UIShell.Model;
using UIShell.ViewModel;
using static MITM_UI.Model.ARPSpoofParticipantsInfo;
using static MITM_UI.Model.ConnectionInfoClass;

namespace MITM_UI.ViewModel.ShellFillerViewModels
{
    public class ARPSpoofViewModel : SingleShellFillerViewModel, INotifyPropertyChanged
    {
        [DllImport("ARPSpoof.dll", EntryPoint = "SniffForHosts", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SniffForHosts(ref ConnectionInfoStruct connectionInfo);

        [DllImport("ARPSpoof.dll", EntryPoint = "ARPSpoof", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ARPSpoof(ref ARPSpoofParticipantsInfoStruct rPSpoofParticipantsInfoStruct);

        [DllImport("ARPSpoof.dll", EntryPoint = "Terminate", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Terminate();

        private static bool isOpen;
        private ObservableCollection<Host> hostsList;
        private Dictionary<string, Host> hosts;
        private List<byte> hostsIPends;
        private GlobalConnectionInfo globalConnectionInfo;

        private string target1;
        private string target2;

        private int sniffForHostsMaxProgressValue = 7;
        private int currentSniffForHostsProgress = 0;
        private bool notSniffing = true;
        private bool notAttack = true;

        private RelayCommand sniffForHostsCommand;

        private RelayCommand setTarget1Command;
        private RelayCommand setTarget2Command;

        private RelayCommand startAttackCommand;

        public ARPSpoofViewModel(GlobalConnectionInfo globalConnectionInfo)
        {
            hostsList = new ObservableCollection<Host>();
            hosts = new Dictionary<string, Host>();
            hostsIPends = new List<byte>();
            this.globalConnectionInfo = globalConnectionInfo;

            target1 = string.Empty;
            target2 = string.Empty;
        }

        public override bool IsOpen
        {
            get { return isOpen; }
            set { isOpen = value; }
        }

        public ObservableCollection<Host> HostsList
        {
            get
            {
                return hostsList;
            }
            set
            {
                hostsList = value;
            }
        }

        public string Target1
        {
            get
            {
                return target1;
            }
            set
            {
                target1 = value;
                RaisePropertyChanged("Target1");
            }
        }

        public string Target2
        {
            get
            {
                return target2;
            }
            set
            {
                target2 = value;
                RaisePropertyChanged("Target2");
            }
        }

        public int SniffForHostsCurrentProgress
        {
            get
            {
                return currentSniffForHostsProgress;
;
            }
            set
            {
                currentSniffForHostsProgress = value;
                RaisePropertyChanged("SniffForHostsCurrentProgress");
            }
        }

        public int SniffForHostsMaxProgressValue
        {
            get
            {
                return sniffForHostsMaxProgressValue;
            }
            set
            {
                sniffForHostsMaxProgressValue = value;
                RaisePropertyChanged("SniffForHostsMaxProgressValue");
            }
        }

        public bool NotSniffing
        {
            get
            {
                return notSniffing;
            }
            set
            {
                notSniffing = value;
                RaisePropertyChanged("NotSniffing");
            }
        }

        public bool NotAttack
        {
            get
            {
                return notAttack;
            }
            set
            {
                notAttack = value;
                RaisePropertyChanged("NotAttack");
            }
        }

        public RelayCommand SniffForHostsCommand
        {
            get
            {
                return sniffForHostsCommand ?? new RelayCommand(
                    (parameter) =>
                    {
                        ExecuteSniffForHostsCommand(parameter);
                    });
            }
        }

        public RelayCommand SetTarget1Command
        {
            get
            {
                return setTarget1Command ?? new RelayCommand(
                    (parameter) =>
                    {
                        ExecuteSetTargetCommand((string)parameter, 1);
                    });
            }
        }

        public RelayCommand SetTarget2Command
        {
            get
            {
                return setTarget2Command ?? new RelayCommand(
                    (parameter) =>
                    {
                        ExecuteSetTargetCommand((string)parameter, 2);
                    });
            }
        }

        public RelayCommand StartAttackCommand
        {
            get
            {
                return startAttackCommand ?? new RelayCommand(
                    (parameter) =>
                    {
                        ExecuteStartAttackCommand(parameter);
                    });
            }
        }

        public void ExecuteSniffForHostsCommand(object parameter)
        {
            byte[] hosts = new byte[200];

            ConnectionInfoStruct connectionInfoStruct = new ConnectionInfoStruct();
            connectionInfoStruct.SubnetMask = globalConnectionInfo.SubnetMask;
            connectionInfoStruct.DefaultGateway = new byte[4] { 192, 168, 0, 1};
            connectionInfoStruct.IPAddress = globalConnectionInfo.IPAddress;
            connectionInfoStruct.MACAddress = globalConnectionInfo.MACAddress;
            connectionInfoStruct.Name = globalConnectionInfo.Name;
            connectionInfoStruct.Hosts = new byte[200];
            connectionInfoStruct.Sleep = (this.sniffForHostsMaxProgressValue-2)*1000;

            NotSniffing = false;
            Task.Factory.StartNew(() => SniffForHosts(connectionInfoStruct));

            SniffForHostsCurrentProgress = 0;
            Task.Factory.StartNew(() => ProgressBarChange());
        }

        public void ExecuteSetTargetCommand(string address, int targetNum)
        {
            switch (targetNum)
            {
                case 1:
                    this.Target1 = address;
                    break;
                case 2:
                    this.Target2 = address;
                    break;
            }
        }

        public void ExecuteStartAttackCommand(object parameter)
        {
            if ((string)parameter == "Start Attack")
            {
                //foreach (Attack a in ActiveAttacksViewModel.ActiveAttacks)
                //{
                //    if ((a.Target1 == target1 && a.Target2 == target2) || (a.Target1 == target2 && a.Target2 == target1))
                //    {
                //        MessageBox.Show("An attack on the chosen IP addresses is already in progress. See View->Active Attacks for details.", "Attack Already in Progress", MessageBoxButton.OK);
                //        return;
                //    }
                //}

                ARPSpoofParticipantsInfoStruct aRPSpoofParticipantsInfoStruct = new ARPSpoofParticipantsInfoStruct();

                Host host = null;
                this.hosts.TryGetValue(this.target1, out host);

                if (host != null)
                {
                    aRPSpoofParticipantsInfoStruct.Target1IPAddress = host.IPAddressArray;
                    aRPSpoofParticipantsInfoStruct.Target1MACAddress = host.MACAddressArray;
                }
                else
                {
                    return;
                }

                host = null;
                this.hosts.TryGetValue(this.target2, out host);

                if (host != null)
                {
                    aRPSpoofParticipantsInfoStruct.Target2IPAddress = host.IPAddressArray;
                    aRPSpoofParticipantsInfoStruct.Target2MACAddress = host.MACAddressArray;
                }
                else
                {
                    return;
                }

                aRPSpoofParticipantsInfoStruct.MyIPAddress = globalConnectionInfo.IPAddress;
                aRPSpoofParticipantsInfoStruct.MyMACAddress = globalConnectionInfo.MACAddress;

                aRPSpoofParticipantsInfoStruct.Name = globalConnectionInfo.Name;

                CancellationTokenSource tokenSource = new CancellationTokenSource();
                Task.Factory.StartNew(() => ARPSpoof(ref aRPSpoofParticipantsInfoStruct), tokenSource.Token);

                //Attack attack = new Attack() { Target1 = this.target1, Target2 = this.target2, Method = AttackMethod.ARP_SPOOF, TokenSource = tokenSource };
                //ActiveAttacksViewModel.ActiveAttacks.Add(attack);

                //this.Target1 = string.Empty;
                //this.Target2 = string.Empty;
                this.NotAttack = false;
            }
            else
            {
                Terminate();
                this.NotAttack = true;
            }
        }

        private void SniffForHosts(ConnectionInfoStruct connectionInfoStruct)
        {
            SniffForHosts(ref connectionInfoStruct);

            int j = 0;
            while (connectionInfoStruct.HostCount > 0)
            {
                if (hostsIPends.Contains(connectionInfoStruct.Hosts[j]) || connectionInfoStruct.Hosts[j] == globalConnectionInfo.IPAddress[3])
                {
                    j += 7;
                    connectionInfoStruct.HostCount--;
                    continue;
                }

                hostsIPends.Add(connectionInfoStruct.Hosts[j]);

                Host host = new Host();
                host.IPAddressArray
                    = new byte[4] {
                        globalConnectionInfo.IPAddress[0],
                        globalConnectionInfo.IPAddress[1],
                        globalConnectionInfo.IPAddress[2],
                        connectionInfoStruct.Hosts[j]
                    };

                for (int k = 0; k < 4; k++)
                {
                    host.IPAddressString += host.IPAddressArray[k];

                    if (k != 3)
                    {
                        host.IPAddressString += ".";
                    }
                }

                host.MACAddressArray = new byte[6];
                for (int k = 0; k < 6; k++)
                {
                    host.MACAddressArray[k] = connectionInfoStruct.Hosts[++j];

                    host.MACAddressString += host.MACAddressArray[k].ToString("X");

                    if (k != 5)
                    {
                        host.MACAddressString += ":";
                    }
                }

                j++;

                Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new Action(() =>
                    {
                        HostsList.Add(host);
                        this.hosts.Add(host.IPAddressString, host);
                    })
                );

                connectionInfoStruct.HostCount--;
            }

            Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new Action(() =>
                    {
                        NotSniffing = true;
                    })
                );
        }

        private async Task ProgressBarChange()
        {
            bool done = false;
            while (!done)
            {
                await Task.Delay(1000);

                await Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new Action(() =>
                    {
                        if (SniffForHostsCurrentProgress == SniffForHostsMaxProgressValue)
                        {
                            done = true;
                        }

                        SniffForHostsCurrentProgress++;
                    })
                );
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

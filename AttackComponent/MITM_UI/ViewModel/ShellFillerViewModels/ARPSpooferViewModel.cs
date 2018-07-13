using MITM_Common;
using MITM_Common.MITM_Service;
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

namespace MITM_UI.ViewModel.ShellFillerViewModels
{
    public class ARPSpoofViewModel : SingleShellFillerViewModel, INotifyPropertyChanged
    {
        MITMServiceProxy mITMServiceProxy = null;

        private static bool isOpen;
        private ObservableCollection<Host> hostsList;
        private Dictionary<string, Host> hosts;
        private List<byte> hostsIPends;

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

        public ARPSpoofViewModel()
        {
            mITMServiceProxy = new MITMServiceProxy(NetTcpBindingCreator.Create());

            hostsList = new ObservableCollection<Host>();
            hosts = new Dictionary<string, Host>();
            hostsIPends = new List<byte>();

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
            NotSniffing = false;

            SniffForHostsCurrentProgress = 0;

            Task.Factory.StartNew(() => ProgressBarChange());

            Task.Factory.StartNew(() => SniffForHosts());
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
                ARPSpoofParticipantsInfo participants = new ARPSpoofParticipantsInfo();

                Host host = null;
                this.hosts.TryGetValue(this.target1, out host);

                if (host != null)
                {
                    participants.Target1IPAddress = host.IPAddressArray;
                    participants.Target1MACAddress = host.MACAddressArray;
                }
                else
                {
                    return;
                }

                host = null;
                this.hosts.TryGetValue(this.target2, out host);

                if (host != null)
                {
                    participants.Target2IPAddress = host.IPAddressArray;
                    participants.Target2MACAddress = host.MACAddressArray;
                }
                else
                {
                    return;
                }

                participants.MyIPAddress = Model.GlobalInfo.Database.GlobalConnectionInfo.IPAddress;
                participants.MyMACAddress = Model.GlobalInfo.Database.GlobalConnectionInfo.MACAddress;

                participants.Name = Model.GlobalInfo.Database.GlobalConnectionInfo.Name;
                
                Task.Factory.StartNew(() => mITMServiceProxy.ARPSpoof(participants));

                this.NotAttack = false;
            }
            else
            {
                mITMServiceProxy.TerminateActiveAttack();
                this.NotAttack = true;
            }
        }

        void SniffForHosts()
        {
            List<Host> hosts = mITMServiceProxy.SniffForHosts();

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, 
            new Action(() =>
            {
                foreach (Host host in hosts)
                {
                    if (!this.hosts.TryGetValue(host.IPAddressString, out Host host1))
                    {
                        this.HostsList.Add(host);
                        this.hosts.Add(host.IPAddressString, host);
                    }
                }

                NotSniffing = true;
            }));
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
    }
}

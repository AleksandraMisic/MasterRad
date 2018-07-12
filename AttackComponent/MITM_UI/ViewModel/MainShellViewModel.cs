using MITM_Common.MITM_Service;
using MITM_Common.PubSub;
using MITM_UI.Extensions.DNP3Extension;
using MITM_UI.Extensions.DNP3Extension.Model;
using MITM_UI.Model;
using MITM_UI.Model.GlobalInfo;
using MITM_UI.View.CustomControls;
using MITM_UI.View.CustomControls.ShellFillers;
using MITM_UI.ViewModel.ShellFillerViewModels;
using PubSub;
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
using System.Windows.Controls;
using System.Windows.Threading;
using UIShell.Model;
using UIShell.View;
using UIShell.ViewModel;

namespace MITM_UI.ViewModel
{
    public class MainShellViewModel : AbstractMainShellViewModel
    {
        [DllImport("ARPSpoof.dll", EntryPoint = "GetNetworkInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetNetworkInfo(ref ConnectionInfoStruct name);

        #region Private fields

        private Subscriber subscriber;

        private ConnectionInfoViewModel connectionInfoViewModel = null;

        #endregion

        #region Constructors

        public MainShellViewModel()
        {
            TopMenu = new ObservableCollection<UserControl>();
            TopMenu.Add(new TopMenu());

            subscriber = new Subscriber();
            subscriber.Subscribe();
            subscriber.publishConnectionInfoEvent += ConnectionInfoChanged;
        }

        #endregion

        #region Commands

        private RelayCommand openConnectionInfoCommand;
        private RelayCommand openActiveAttacksCommand;

        private RelayCommand openARPSpoofCommand;

        private RelayCommand openDNP3ExtensionCommand;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region CommandAccessors

        public RelayCommand OpenConnectionInfoCommand
        {
            get
            {
                return openConnectionInfoCommand ?? new RelayCommand(
                    (parameter) =>
                    {
                        ExecuteOpenConnectionInfoCommand(parameter);
                    });
            }
        }

        public RelayCommand OpenActiveAttacksCommand
        {
            get
            {
                return openConnectionInfoCommand ?? new RelayCommand(
                    (parameter) =>
                    {
                        ExecuteOpenActiveAttacksCommand(parameter);
                    });
            }
        }

        public RelayCommand OpenARPSpoofCommand
        {
            get
            {
                return openARPSpoofCommand ?? new RelayCommand(
                    (parameter) =>
                    {
                        ExecuteOpenARPSpoofCommand(parameter);
                    });
            }
        }

        public RelayCommand OpenDNP3ExtensionCommand
        {
            get
            {
                return openDNP3ExtensionCommand ?? new RelayCommand(
                    (parameter) =>
                    {
                        ExecuteOpenDNP3ExtensionCommand(parameter);
                    });
            }
        }

        #endregion

        #region Execute open commands

        private void ExecuteOpenConnectionInfoCommand(object parameter)
        {
            ConnectionInfoViewModel civm = new ConnectionInfoViewModel();
            if (civm.IsOpen == false)
            {
                civm.IsOpen = true;

                ConnectionInfo connectionInfo = new ConnectionInfo();

                connectionInfo.DataContext = civm;

                ShellFillerShell sfs = new ShellFillerShell() { DataContext = this };

                sfs.MainScroll.Content = connectionInfo;
                sfs.Header.Text = (string)parameter;

                PlaceOrFocusControlInShell(ShellPosition.LEFT, sfs, false, null);

                this.connectionInfoViewModel = civm;

                return;
            }

            PlaceOrFocusControlInShell(ShellPosition.LEFT, null, true, "Connection Info");
        }

        private void ExecuteOpenActiveAttacksCommand(object parameter)
        {
            ActiveAttacksViewModel aavm = new ActiveAttacksViewModel();
            if (aavm.IsOpen == false)
            {
                aavm.IsOpen = true;

                ActiveAttacks activeAttacks = new ActiveAttacks();

                activeAttacks.DataContext = aavm;

                ShellFillerShell sfs = new ShellFillerShell() { DataContext = this };

                sfs.MainScroll.Content = activeAttacks;
                sfs.Header.Text = (string)parameter;

                PlaceOrFocusControlInShell(ShellPosition.BOTTOM, sfs, false, null);

                return;
            }

            PlaceOrFocusControlInShell(ShellPosition.LEFT, null, true, "Connection Info");
        }

        private void ExecuteOpenARPSpoofCommand(object parameter)
        {
            ARPSpoofViewModel asvm = new ARPSpoofViewModel();
            if (asvm.IsOpen == false)
            {
                asvm.IsOpen = true;

                ARPSpoof connectionInfo = new ARPSpoof();

                connectionInfo.DataContext = asvm;

                ShellFillerShell sfs = new ShellFillerShell() { DataContext = this };

                sfs.MainScroll.Content = connectionInfo;
                sfs.Header.Text = (string)parameter;

                PlaceOrFocusControlInShell(ShellPosition.CENTER, sfs, false, null);

                return;
            }

            PlaceOrFocusControlInShell(ShellPosition.LEFT, null, true, "Connection Info");
        }

        private void ExecuteOpenDNP3ExtensionCommand(object parameter)
        {
            DNP3ExtensionMainWindow dNP3ExtensionMainWindow = new DNP3ExtensionMainWindow();
            dNP3ExtensionMainWindow.Show();
        }

        #endregion

        void ConnectionInfoChanged(GlobalConnectionInfo connectionInfo)
        {
            Database.GlobalConnectionInfo = connectionInfo;

            if (connectionInfoViewModel.IsOpen)
            {
                connectionInfoViewModel.CheckForConnectionChange();
            }
        }
    }
}

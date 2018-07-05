using MITM_UI.Extensions.DNP3Extension.Model;
using MITM_UI.Extensions.View;
using MITM_UI.View.CustomControls.ShellFillers;
using MITM_UI.ViewModel.ShellFillerViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using UIShell.Model;
using UIShell.View;
using UIShell.ViewModel;

namespace MITM_UI.Extensions.DNP3Extension.ViewModel
{
    public class MainShellViewModel : AbstractMainShellViewModel
    {
        public MainShellViewModel()
        {
            TopMenu = new ObservableCollection<UserControl>();
            TopMenu.Add(new TopMenu());
        }

        #region Commands

        private RelayCommand openConnectionInfoCommand;

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

        #endregion

        #region Execute open commands

        private void ExecuteOpenConnectionInfoCommand(object parameter)
        {
            //ConnectionInfoViewModel civm = new ConnectionInfoViewModel(this.globalConnectionInfo);
            //if (civm.IsOpen == false)
            //{
            //    civm.IsOpen = true;

            //    ConnectionInfo connectionInfo = new ConnectionInfo();

            //    connectionInfo.DataContext = civm;

            //    TaskFactory taskFactory = new TaskFactory();
            //    taskFactory.StartNew(new Action(civm.GetNetworkInfo));

            //    ShellFillerShell sfs = new ShellFillerShell() { DataContext = this };

            //    sfs.MainScroll.Content = connectionInfo;
            //    sfs.Header.Text = (string)parameter;

            //    PlaceOrFocusControlInShell(ShellPosition.LEFT, sfs, false, null);

            //    return;
            //}

            //PlaceOrFocusControlInShell(ShellPosition.LEFT, null, true, "Connection Info");
        }

        #endregion
    }
}

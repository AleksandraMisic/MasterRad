using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UIShell.Model;

namespace UIShell.ViewModel
{
    public class ShellFillerShellViewModel
    {
        private RelayCommand closeControlCommand;

        public RelayCommand CloseControlCommand
        {
            get
            {
                return closeControlCommand ?? new RelayCommand(
                    (parameter) =>
                    {
                        ExecuteCloseControlCommand(parameter);
                    });
            }
        }

        private void ExecuteCloseControlCommand(object parameter)
        {
            var header = ((object[])parameter)[0];
            var position = ((object[])parameter)[1];
            MainShellViewModel mainShellViewModel = ((object[])parameter)[3] as MainShellViewModel;

            var properties = mainShellViewModel.ShellProperties[(ShellPosition)position];

            properties.TabControlTabs.Remove(properties.TabControlTabs.Where(tab => tab.Header == header).First());

            if (properties.TabControlTabs.Count == 0)
            {
                properties.TabControlVisibility = Visibility.Collapsed;
            }

            try
            {
                SingleShellFillerViewModel viewModel = (SingleShellFillerViewModel)((object[])parameter)[2];
                viewModel.IsOpen = false;
            }
            catch { }
            finally
            {
                ((object[])parameter)[2] = null;
            }
        }
    }
}

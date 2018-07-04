using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using UIShell.ViewModel;

namespace DNP3Outstation.ViewModel
{
    public class DevicesExplorerViewModel : SingleShellFillerViewModel
    {
        private static bool isOpen;
        private ObservableCollection<TreeViewItem> devices;

        public DevicesExplorerViewModel()
        {
            devices = new ObservableCollection<TreeViewItem>();
        }

        public ObservableCollection<TreeViewItem> Devices
        {
            get
            {
                return devices;
            }
            set
            {
                devices = value;
            }
        }

        public override bool IsOpen
        {
            get { return isOpen; }
            set { isOpen = value; }
        }
    }
}

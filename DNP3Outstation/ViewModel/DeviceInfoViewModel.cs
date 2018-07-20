using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using UIShell.Model;
using UIShell.ViewModel;

namespace DNP3Outstation.ViewModel
{
    public class DeviceInfoViewModel : SingleShellFillerViewModel
    {
        private static bool isOpen;
        private static ShellPosition position;

        public static ShellPosition Position
        {
            get { return position; }
            set { position = value; }
        }

        public DeviceInfoViewModel()
        {
            Position = ShellPosition.LEFT;
        }

        public override bool IsOpen
        {
            get { return isOpen; }
            set { isOpen = value; }
        }
    }
}

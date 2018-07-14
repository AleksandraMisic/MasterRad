using DispatcherApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIShell.Model;
using UIShell.ViewModel;

namespace DispatcherApp.ViewModel.ShellFillerModelViews
{
    public class ReportExplorerModelView : SingleShellFillerViewModel
    {
        private static bool isOpen;
        private static ShellPosition position;

        public override bool IsOpen
        {
            get { return isOpen; }
            set { isOpen = value; }
        }

        public static ShellPosition Position
        {
            get { return position; }
            set { position = value; }
        }

        static ReportExplorerModelView()
        {
            Position = ShellPosition.CENTER;
        }

        public ReportExplorerModelView()
        {

        }
    }
}

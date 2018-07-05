using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIShell.ViewModel;

namespace MITM_UI.ViewModel.ShellFillerViewModels
{
    public class ActiveAttacksViewModel : SingleShellFillerViewModel
    {
        private bool isOpen;

        public override bool IsOpen
        {
            get { return isOpen; }
            set { isOpen = value; }
        }
    }
}

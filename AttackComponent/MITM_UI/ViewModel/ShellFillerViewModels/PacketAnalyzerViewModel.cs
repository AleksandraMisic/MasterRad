using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIShell.ViewModel;

namespace MITM_UI.ViewModel.ShellFillerViewModels
{
    public class PacketAnalyzerViewModel : SingleShellFillerViewModel
    {
        private static bool isOpen;

        private const int maxNumeOfPacketsPerPage = 100;
        private const int maxNumOfPages = 100;

        private string currentFilePath = string.Empty;

        public override bool IsOpen
        {
            get { return isOpen; }
            set { isOpen = value; }
        }
    }
}

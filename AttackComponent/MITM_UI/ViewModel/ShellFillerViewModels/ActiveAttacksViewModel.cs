using MITM_UI.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public static ObservableCollection<Attack> ActiveAttacks { get; set; }

        static ActiveAttacksViewModel()
        {
            ActiveAttacks = new ObservableCollection<Attack>();
        }
    }
}

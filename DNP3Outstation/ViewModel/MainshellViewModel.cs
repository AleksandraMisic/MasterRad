using DNP3Outstation.View;
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
    public class MainshellViewModel : AbstractMainShellViewModel
    {
        public MainshellViewModel()
        {
            TopMenu = new ObservableCollection<UserControl>();
            TopMenu.Add(new TopMenu());
        }
    }
}

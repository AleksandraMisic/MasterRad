using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UIShell.ViewModel;

namespace UIShell.View
{
    /// <summary>
    /// Interaction logic for ShellFillerShell.xaml
    /// </summary>
    public partial class ShellFillerShell : UserControl
    {
        public ShellFillerShell()
        {
            InitializeComponent();
            this.DataContext = new ShellFillerShellViewModel();
        }
    }
}

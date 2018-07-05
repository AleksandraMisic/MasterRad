using MITM_UI.Extensions.DNP3Extension.ViewModel;
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
using System.Windows.Shapes;

namespace MITM_UI.Extensions.DNP3Extension
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class DNP3ExtensionMainWindow : Window
    {
        public DNP3ExtensionMainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainShellViewModel();
        }
    }
}

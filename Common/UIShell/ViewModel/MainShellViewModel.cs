using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using UIShell.Model;

namespace UIShell.ViewModel
{
    public class MainShellViewModel : INotifyPropertyChanged
    {
        #region Private fields

        private Dictionary<ShellPosition, ShellFillerShellProperties> shellProperties;

        #endregion

        #region Public properties

        public Dictionary<ShellPosition, ShellFillerShellProperties> ShellProperties
        {
            get
            {
                return shellProperties;
            }
            set
            {
                shellProperties = value;
                RaisePropertyChanged("ShellProperties");
            }
        }

        #endregion

        #region Constructors

        public MainShellViewModel()
        {
            shellProperties = new Dictionary<ShellPosition, ShellFillerShellProperties>();

            foreach (var position in Enum.GetValues(typeof(ShellPosition)))
            {
                ShellProperties.Add((ShellPosition)position, new ShellFillerShellProperties());
            }
        }

        #endregion

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DispatcherApp.Model
{
    public class ShellFillerShellProperties : INotifyPropertyChanged
    {
        private ObservableCollection<TabItem> tabControlTabs;
        private int tabControlIndex;
        private Visibility tabControlVisibility;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<TabItem> TabControlTabs
        {
            get
            {
                return tabControlTabs;
            }
            set
            {
                tabControlTabs = value;
            }
        }

        public int TabControlIndex
        {
            get
            {
                return tabControlIndex;
            }
            set
            {
                tabControlIndex = value;
                RaisePropertyChanged("TabControlIndex");
            }
        }

        public Visibility TabControlVisibility
        {
            get
            {
                return tabControlVisibility;
            }
            set
            {
                tabControlVisibility = value;
                RaisePropertyChanged("TabControlVisibility");
            }
        }

        public ShellFillerShellProperties()
        {
            TabControlTabs = new ObservableCollection<TabItem>();
            TabControlIndex = 0;
            tabControlVisibility = Visibility.Collapsed;
        }

        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}

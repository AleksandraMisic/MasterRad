using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using UIShell.View;

namespace UIShell.ViewModel
{
    public abstract class AbstractMainShellViewModel : INotifyPropertyChanged
    {
        #region Fields

        protected Dictionary<ShellPosition, ShellFillerShellProperties> shellProperties;

        #endregion

        #region Properties

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

        public ObservableCollection<UserControl> TopMenu { get; set; }

        #endregion

        #region Constructors

        public AbstractMainShellViewModel()
        {
            ShellProperties = new Dictionary<ShellPosition, ShellFillerShellProperties>();

            foreach (var position in Enum.GetValues(typeof(ShellPosition)))
            {
                ShellProperties.Add((ShellPosition)position, new ShellFillerShellProperties());
            }
        }

        #endregion

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion

        protected void PlaceOrFocusControlInShell(ShellPosition position, ShellFillerShell sfs, bool isFocus, string parameter)
        {
            var currentTabControl = ShellProperties[position];

            if (!isFocus)
            {
                TabItem tabItem = new TabItem() { Header = sfs.Header.Text };

                tabItem.Content = sfs;
                tabItem.Header = sfs.Header.Text;

                currentTabControl.TabControlTabs.Add(tabItem);
                currentTabControl.TabControlIndex = currentTabControl.TabControlTabs.Count - 1;
                RaisePropertyChanged("ShellProperties");

                if (position != ShellPosition.CENTER)
                {
                    currentTabControl.TabControlVisibility = Visibility.Visible;
                }
                else
                {
                    sfs.Header.Text = "";
                }
            }
            else
            {
                int i = 0;
                for (i = 0; i < currentTabControl.TabControlTabs.Count; i++)
                {
                    if ((string)currentTabControl.TabControlTabs[i].Header == parameter)
                    {
                        break;
                    }
                }

                currentTabControl.TabControlIndex = i;
            }
        }
    }
}

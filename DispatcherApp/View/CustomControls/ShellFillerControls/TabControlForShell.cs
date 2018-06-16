using DispatcherApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DispatcherApp.View.CustomControls.ShellFillerControls
{
    public class TabControlForShell : TabControl
    {
        public static readonly DependencyProperty position =
            DependencyProperty.Register("Position", typeof(ShellPosition),
            typeof(TabControlForShell));

        public ShellPosition Position
        {
            get
            {
                return (ShellPosition)GetValue(position);
            }
            set
            {
                SetValue(position, value);
            }
        }
    }
}

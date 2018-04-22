using DispatcherApp.Model;
using DispatcherApp.ViewModel.ShellFillerViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DispatcherApp.ViewModel.ShellFillerModelViews
{
    public class NetworkViewViewModel
    {
        private static ShellPosition position;

        public static ShellPosition Position
        {
            get { return position; }
            set { position = value; }
        }

        static NetworkViewViewModel()
        {
            Position = ShellPosition.CENTER;
        }

        public NetworkViewViewModel()
        {

        }
    }
}

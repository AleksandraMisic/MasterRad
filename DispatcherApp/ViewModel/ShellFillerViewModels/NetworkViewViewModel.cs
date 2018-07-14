using DispatcherApp.Model;
using DispatcherApp.Model.DrawingAlgorithms.Schematic.RadialNetwork;
using DMSCommon.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TransactionManagerContract;
using TransactionManagerContract.ClientDMS;
using UIShell.Model;

namespace DispatcherApp.ViewModel.ShellFillerModelViews
{
    public class NetworkViewViewModel
    {
        private static ShellPosition position;

        private ObservableCollection<UIElement> itemsSourceForCanvas;

        private ClientDMSProxy cdClient;

        public static ShellPosition Position
        {
            get { return position; }
            set { position = value; }
        }

        public ObservableCollection<UIElement> ItemsSourceForCanvas
        {
            get { return itemsSourceForCanvas; }
            set { itemsSourceForCanvas = value; }
        }

        static NetworkViewViewModel()
        {
            Position = ShellPosition.CENTER;
        }

        public NetworkViewViewModel()
        {
            itemsSourceForCanvas = new ObservableCollection<UIElement>();
            cdClient = new ClientDMSProxy();
        }

        public void GetNetwork(string mrid)
        {
            List<Element> list = cdClient.GetNetwork(mrid);

            SimpleWidthDivider simpleWidthDivider = new SimpleWidthDivider();
            simpleWidthDivider.DrawNetwork(list, 10, this.itemsSourceForCanvas);
        }
    }
}

using DispatcherApp.Model;
using DispatcherApp.ViewModel.ShellFillerViewModels;
using DMSCommon.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionManagerContract;
using TransactionManagerContract.ClientDMS;

namespace DispatcherApp.ViewModel.ShellFillerModelViews
{
    public class NetworkViewViewModel
    {
        private static ShellPosition position;
        private ClientDMSProxy cdClient;

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
            cdClient = new ClientDMSProxy();
        }

        public void GetNetwork(string mrid)
        {
            List<Element> list = cdClient.GetNetwork(mrid);
        }
    }
}

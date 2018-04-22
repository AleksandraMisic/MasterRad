using DispatcherApp.Model;
using DispatcherApp.View;
using DispatcherApp.ViewModel.ShellFillerViewModels;
using DMSCommon.Model;
using OMSCommon;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using TransactionManagerContract;

namespace DispatcherApp.ViewModel.ShellFillerModelViews
{
    public class NetworkExplorerViewModel : ShellFillerViewModel
    {
        private static bool isOpen;
        private static ShellPosition position;
        private ObservableCollection<Button> sources;

        private IOMSClient proxyToOMS;

        public override bool IsOpen
        {
            get { return isOpen; }
            set { isOpen = value; }
        }

        public static ShellPosition Position
        {
            get { return position; }
            set { position = value; }
        }

        public ObservableCollection<Button> Sources
        {
            get { return sources; }
            set { sources = value; }
        }

        static NetworkExplorerViewModel()
        {
            Position = ShellPosition.LEFT;
        }

        public IOMSClient ProxyToOMS
        {
            get { return proxyToOMS; }
            set { proxyToOMS = value; }
        }

        public NetworkExplorerViewModel()
        {
        }

        public NetworkExplorerViewModel(ObservableCollection<Button> sources)
        {
            Sources = sources;
        }

        public void GetAllSources(MainShellViewModel viewModel)
        {
            this.sources = new ObservableCollection<Button>();

            if (proxyToOMS == null)
            {
                CreateChannel();
            }

            List<Source> sourcesList = new List<Source>();
            try
            {
                sourcesList = ProxyToOMS.GetSources();
            }
            catch (Exception e)
            {
                return;
            }
            
            foreach (Source source in sourcesList)
            {
                Button but = new Button() { Content = source.MRID, DataContext = viewModel, Command = viewModel.OpenNetworkViewCommand, CommandParameter = source.MRID };
                this.Sources.Add(but);
            }
        }

        private void CreateChannel()
        {
            ChannelFactory<IOMSClient> factoryToOMS = new ChannelFactory<IOMSClient>(NetTcpBindingCreator.Create(),
                new EndpointAddress("net.tcp://localhost:6080/TransactionManagerService"));
            ProxyToOMS = factoryToOMS.CreateChannel();
        }
    }
}

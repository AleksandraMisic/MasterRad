using DispatcherApp.Model;
using DMSCommon.Model;
using IMSContract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TransactionManagerContract;
using UIShell.Model;
using UIShell.ViewModel;

namespace DispatcherApp.ViewModel.ShellFillerModelViews
{
    public class IncidentExplorerViewModel : SingleShellFillerViewModel
    {
        private static bool isOpen;
        private static ShellPosition position;
        private ObservableCollection<IncidentReport> incidentReports;

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

        public ObservableCollection<IncidentReport> IncidentReports
        {
            get { return incidentReports; }
            set { incidentReports = value; }
        }

        static IncidentExplorerViewModel()
        {
            Position = ShellPosition.BOTTOM;
        }

        public IOMSClient ProxyToOMS
        {
            get { return proxyToOMS; }
            set { proxyToOMS = value; }
        }

        public IncidentExplorerViewModel()
        {

        }

        public void GetAllIncidentReports()
        {
            this.IncidentReports = new ObservableCollection<IncidentReport>();

            if (proxyToOMS == null)
            {
                CreateChannel();
            }

            List<IncidentReport> reportsList = null;

            try
            {
                reportsList = ProxyToOMS.GetAllIncidentReports();
            }
            catch (Exception e)
            {
                return;
            }

            foreach (IncidentReport report in reportsList)
            {
                this.IncidentReports.Add(report);
            }
        }

        private void CreateChannel()
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.CloseTimeout = new TimeSpan(1, 0, 0, 0);
            binding.OpenTimeout = new TimeSpan(1, 0, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0, 0);
            binding.MaxReceivedMessageSize = Int32.MaxValue;

            ChannelFactory<IOMSClient> factoryToOMS = new ChannelFactory<IOMSClient>(binding,
                new EndpointAddress("net.tcp://localhost:6080/TransactionManagerService"));
            ProxyToOMS = factoryToOMS.CreateChannel();
        }
    }
}

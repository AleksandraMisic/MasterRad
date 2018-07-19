using DispatcherApp.Model;
using DispatcherApp.View;
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
using TransactionManagerContract.ClientDMS;
using UIShell.Model;
using UIShell.View;
using UIShell.ViewModel;

namespace DispatcherApp.ViewModel.ShellFillerModelViews
{
    public class NetworkExplorerViewModel : SingleShellFillerViewModel
    {
        private static bool isOpen;
        private static ShellPosition position;
        private ObservableCollection<Button> sources;

        RelayCommand openNetworkViewCommand;

        private ClientDMSProxy cgProxy;

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

        public NetworkExplorerViewModel()
        {
            cgProxy = new ClientDMSProxy();
        }

        public NetworkExplorerViewModel(ObservableCollection<Button> sources)
        {
            Sources = sources;
        }

        public RelayCommand OpenNetworkViewCommand
        {
            get
            {
                return openNetworkViewCommand ?? new RelayCommand(
                    (parameter) =>
                    {
                        ExecuteOpenNetworkViewCommand(parameter);
                    });
            }
        }

        private void ExecuteOpenNetworkViewCommand(object parameter)
        {
            bool isOpen = false;

            if (!MainShellViewModel.IsSourceOpen.TryGetValue((string)parameter, out isOpen))
            {
                MainShellViewModel.IsSourceOpen.Add((string)parameter, true);
            }

            if (!isOpen)
            {
                MainShellViewModel.IsSourceOpen[(string)parameter] = true;

                NetworkViewControl networkViewExplorer = new NetworkViewControl();
                NetworkViewViewModel nvevm = new NetworkViewViewModel();
                networkViewExplorer.DataContext = nvevm;

                nvevm.GetNetwork((string)parameter);

                ShellFillerShell sfs = new ShellFillerShell();

                sfs.MainScroll.Content = networkViewExplorer;
                sfs.Header.Text = (string)parameter;

                new MainShellViewModel().PlaceOrFocusControlInShell(NetworkViewViewModel.Position, sfs, false, null);

                return;
            }

            new MainShellViewModel().PlaceOrFocusControlInShell(NetworkViewViewModel.Position, null, true, (string)parameter);
        }

        public void GetAllSources()
        {
            this.Sources = new ObservableCollection<Button>();

            foreach (Source source in LocalCache.Sources.Values)
            {
                Button but = new Button() { Content = source.MRID, DataContext = this, Command = OpenNetworkViewCommand, CommandParameter = source.MRID };
                this.Sources.Add(but);
            }

            List<Source> sourcesList = new List<Source>();
            try
            {
                sourcesList = cgProxy.GetAllSources();
            }
            catch (Exception e)
            {
                return;
            }
            
            foreach (Source source in sourcesList)
            {
                if (!LocalCache.Sources.TryGetValue(source.MRID, out Source source1))
                {
                    LocalCache.Sources.Add(source.MRID, source);

                    Button but = new Button() { Content = source.MRID, DataContext = this, Command = OpenNetworkViewCommand, CommandParameter = source.MRID };
                    this.Sources.Add(but);
                }
            }
        }
    }
}

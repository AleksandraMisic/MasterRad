using DNP3ConfigParser.Parsers;
using DNP3DataPointsModel;
using DNP3Outstation.Communication;
using DNP3Outstation.Model;
using DNP3Outstation.View;
using DNP3Outstation.View.ShellFillers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Linq;
using UIShell.Model;
using UIShell.View;
using UIShell.ViewModel;

namespace DNP3Outstation.ViewModel
{
    public class MainshellViewModel : AbstractMainShellViewModel
    {
        private Database database = new Database();

        private UniversalConfigurationParser configParser;
        private string configPath;

        private RelayCommand openDeviceInfoCommand;
        private RelayCommand openDataPointsCommand;

        public MainshellViewModel()
        {
            TopMenu = new ObservableCollection<UserControl>();
            TopMenu.Add(new TopMenu());

            configPath = Directory.GetCurrentDirectory() + "..\\..\\..\\open_dnp3_slave.xml";

            XDocument document = XDocument.Load(configPath);
            XNamespace ns = document.Root.GetDefaultNamespace();

            switch (ns.NamespaceName)
            {
                case "http://www.dnp3.org/DNP3/DeviceProfile/Jan2010":

                    configParser = new DNP3DeviceProfileJan2010Parser(document);
                    ((DNP3DeviceProfileJan2010Parser)configParser).Parse();

                    lock (Database.DatabaseLock)
                    {
                        foreach (AnalogInputPoint analog in ((DNP3DeviceProfileJan2010Parser)configParser).Configuration.DataPointsListConfiguration.AnalogInputPoints)
                        {
                            Database.AnalogInputPoints.Add(analog);
                        }
                    }

                    CommunicationEngine communicationEngine = new CommunicationEngine(((DNP3DeviceProfileJan2010Parser)configParser).Configuration.NetworkConfiguration.IpAddress, 20000);
                    break;
            }

            Task.Factory.StartNew(() => AnalogPointsSimulation());
        }

        public RelayCommand OpenDeviceInfoCommand
        {
            get
            {
                return openDeviceInfoCommand ?? new RelayCommand(
                    (parameter) =>
                    {
                        ExecuteOpenDeviceInfoCommand();
                    });
            }
        }

        public RelayCommand OpenDataPointsCommand
        {
            get
            {
                return openDataPointsCommand ?? new RelayCommand(
                    (parameter) =>
                    {
                        ExecuteOpenDataPointsCommand();
                    });
            }
        }

        private void ExecuteOpenDeviceInfoCommand()
        {
            DeviceInfoViewModel divm = new DeviceInfoViewModel();

            if (divm.IsOpen == false)
            {
                divm.IsOpen = true;

                DeviceInfo deviceInfo = new DeviceInfo();
                deviceInfo.DataContext = divm;

                deviceInfo.DeviceSubTree.DataContext = ((DNP3DeviceProfileJan2010Parser)configParser).Configuration.DeviceConfiguration;
                deviceInfo.NetworkSubTree.DataContext = ((DNP3DeviceProfileJan2010Parser)configParser).Configuration.NetworkConfiguration;

                ShellFillerShell sfs = new ShellFillerShell() { DataContext = this };

                sfs.MainScroll.Content = deviceInfo;
                sfs.Header.Text = "Device Info";

                PlaceOrFocusControlInShell(DeviceInfoViewModel.Position, sfs, false, null);

                return;
            }

            PlaceOrFocusControlInShell(DeviceInfoViewModel.Position, null, true, "Device Info");
        }

        private void ExecuteOpenDataPointsCommand()
        {
            DataPointsViewModel dpvm = new DataPointsViewModel();

            if (dpvm.IsOpen == false)
            {
                dpvm.IsOpen = true;

                DataPoints dataPoints = new DataPoints();
                dataPoints.DataContext = dpvm;
                dataPoints.AnalogInputs.ItemsSource = dpvm.AnalogInputPoints;

                ShellFillerShell sfs = new ShellFillerShell() { DataContext = this };

                sfs.MainScroll.Content = dataPoints;
                sfs.Header.Text = "Data Points";

                PlaceOrFocusControlInShell(DataPointsViewModel.Position, sfs, false, null);

                return;
            }

            PlaceOrFocusControlInShell(DataPointsViewModel.Position, null, true, "Data Points");
        }

        void AnalogPointsSimulation()
        {
            int maxValue = 10000;

            while (true)
            {
                lock (Database.DatabaseLock)
                {
                    foreach (AnalogInputPoint analog in Database.AnalogInputPoints)
                    {
                        if (analog.Value < maxValue)
                        {
                            analog.Value += 2;
                            continue;
                        }

                        analog.Value = 0;
                    }
                }

                Thread.Sleep(3000);
            }
        }
    }
}

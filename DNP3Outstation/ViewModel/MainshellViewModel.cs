using DNP3ConfigParser.Parsers;
using DNP3Outstation.Communication;
using DNP3Outstation.View;
using DNP3Outstation.View.ShellFillers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
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
        private UniversalConfigurationParser configParser;
        private string configPath;

        private RelayCommand openDeviceInfoCommand;

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

                    CommunicationEngine communicationEngine = new CommunicationEngine(((DNP3DeviceProfileJan2010Parser)configParser).Configuration.NetworkConfiguration.IpAddress, 20000);
                    break;
            }
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
    }
}

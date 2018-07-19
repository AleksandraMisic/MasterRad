using DispatcherApp.Model;
using DispatcherApp.Model.DrawingAlgorithms.Schematic.RadialNetwork;
using DispatcherApp.Model.Measurements;
using DispatcherApp.Model.Properties.DMSProperties;
using DispatcherApp.Model.Properties.NMSProperties;
using DispatcherApp.View;
using DispatcherApp.View.CustomControls.PropertiesControls;
using DispatcherApp.View.ShellFillers;
using DMSCommon.Model;
using FTN.Common;
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
using TransactionManagerContract.ClientNMS;
using UIShell.Model;
using UIShell.View;

namespace DispatcherApp.ViewModel.ShellFillerModelViews
{
    public class NetworkViewViewModel
    {
        private static ShellPosition position;
        private ObservableCollection<UIElement> itemsSourceForCanvas;

        private static ClientDMSProxy cdClient;
        private static ClientNMSProxy cnClient;

        private static RelayCommand openPropertiesCommand;

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

        public static RelayCommand OpenPropertiesCommand
        {
            get
            {
                return openPropertiesCommand ?? new RelayCommand(
                    (parameter) =>
                    {
                        ExecuteOpenPropertiesCommand(parameter);
                    });
            }
        }

        static NetworkViewViewModel()
        {
            Position = ShellPosition.CENTER;
            cdClient = new ClientDMSProxy();
            cnClient = new ClientNMSProxy();
        }

        public NetworkViewViewModel()
        {
            itemsSourceForCanvas = new ObservableCollection<UIElement>();
        }

        public void GetNetwork(string mrid)
        {
            List<Element> list = cdClient.GetNetwork(mrid);

            InitElementProperties(list);

            SimpleWidthDivider simpleWidthDivider = new SimpleWidthDivider(list, InitElementProperties(list));
            simpleWidthDivider.DrawNetwork(10, this.itemsSourceForCanvas);
        }

        public Dictionary<long, ElementProperties> InitElementProperties(List<Element> elements)
        {
            Dictionary<long, ElementProperties> properties = new Dictionary<long, ElementProperties>();

            if (elements != null)
            {
                foreach (Element element in elements)
                {
                    if (element is Switch)
                    {
                        Switch @switch = (Switch)element;
                        SwitchProperties switchProperties = new SwitchProperties()
                        {
                            GID = @switch.ElementGID,
                            IsEnergized = @switch.IsEnergized,
                            IsUnderScada = @switch.UnderSCADA,
                            Incident = @switch.Incident,
                            CanCommand = @switch.CanCommand,
                            ParentGid = @switch.End1
                        };

                        properties.Add(switchProperties.GID, switchProperties);
                    }
                    else if (element is Consumer)
                    {
                        Consumer consumer = (Consumer)element;
                        ConsumerProperties consumerProperties = new ConsumerProperties()
                        {
                            GID = consumer.ElementGID,
                            IsEnergized = consumer.IsEnergized,
                            IsUnderScada = consumer.UnderSCADA,
                            //Call = consumer.Call
                        };

                        properties.Add(consumerProperties.GID, consumerProperties);
                    }
                    else if (element is Source)
                    {
                        Source source = (Source)element;
                        SourceProperties sourceProperties = new SourceProperties()
                        {
                            GID = source.ElementGID,
                            IsEnergized = source.IsEnergized,
                            IsUnderScada = source.UnderSCADA
                        };

                        properties.Add(sourceProperties.GID, sourceProperties);
                    }
                    else if (element is ACLine)
                    {
                        ACLine acLine = (ACLine)element;
                        ACLineProperties acLineProperties = new ACLineProperties()
                        {
                            GID = acLine.ElementGID,
                            IsEnergized = acLine.IsEnergized,
                            IsUnderScada = acLine.UnderSCADA
                        };

                        properties.Add(acLineProperties.GID, acLineProperties);
                    }
                    else if (element is Node)
                    {
                        Node node = (Node)element;
                        NodeProperties nodeProperties = new NodeProperties()
                        {
                            GID = node.ElementGID,
                            IsEnergized = node.IsEnergized
                        };

                        properties.Add(nodeProperties.GID, nodeProperties);
                    }
                }
            }

            foreach (Model.Properties.DMSProperties.ElementProperties property in properties.Values)
            {
                if (property is SwitchProperties)
                {
                    properties.TryGetValue(((SwitchProperties)property).ParentGid, out ElementProperties elementProperties);

                    if (elementProperties != null)
                    {
                        ((SwitchProperties)property).Parent = elementProperties;
                    }
                }
            }

            return properties;
        }

        private static void ExecuteOpenPropertiesCommand(object parameter)
        {
            ResourceDescription rd = cnClient.GetStaticDataForElement((long)parameter);

            PropertiesControl propertiesControl = new PropertiesControl();

            List<DigitalMeasurement> digitalMeasurements = new List<DigitalMeasurement>();
            List<AnalogMeasurement> analogMeasurements = new List<AnalogMeasurement>();

            if (rd != null)
            {
                StaticProperties staticProperties = new StaticProperties();
                staticProperties.ReadFromResourceDescription(rd);

                GeneralStaticPropertiesControl generalStaticPropertiesControl = new GeneralStaticPropertiesControl()
                {
                    DataContext = staticProperties
                };

                propertiesControl.StaticProperties.Content = generalStaticPropertiesControl;

                if (rd.ContainsProperty(ModelCode.PSR_MEASUREMENTS))
                {
                    List<long> measurementGids = rd.GetProperty(ModelCode.PSR_MEASUREMENTS).AsLongs();

                    foreach (long meas in measurementGids)
                    {
                        rd = cnClient.GetStaticDataForElement(meas);

                        short type = ModelCodeHelper.ExtractTypeFromGlobalId(meas);

                        if (type == (short)DMSType.DISCRETE)
                        {
                            DigitalMeasurement digitalMeasurement = new DigitalMeasurement();
                            digitalMeasurement.ReadFromResourceDescription(rd);

                            digitalMeasurements.Add(digitalMeasurement);
                        }
                        else if (type == (short)DMSType.ANALOG)
                        {
                            AnalogMeasurement analogMeasurement = new AnalogMeasurement();
                            analogMeasurement.ReadFromResourceDescription(rd);

                            analogMeasurements.Add(analogMeasurement);
                        }
                    }
                }
            }

            PropertiesModelView propertiesModelView = new PropertiesModelView();

            if (digitalMeasurements.Count == 0 && analogMeasurements.Count == 0)
            {
                propertiesModelView.MeasurementVisibility = Visibility.Collapsed;
            }
            else
            {
                propertiesModelView.MeasurementVisibility = Visibility.Visible;

                propertiesControl.Measurements.Content = new MeasurementsControl()
                {
                    DataContext = propertiesModelView
                };

                if (digitalMeasurements.Count > 0)
                {
                    foreach (DigitalMeasurement measurement in digitalMeasurements)
                    {
                        DiscreteMeasurementControl discreteMeasurementControl = new DiscreteMeasurementControl()
                        {
                            DataContext = measurement
                        };

                        propertiesModelView.DigitalControls.Add(discreteMeasurementControl);
                    }

                    propertiesModelView.DigitalMeasurementVisibility = Visibility.Visible;
                }
                else
                {
                    propertiesModelView.DigitalMeasurementVisibility = Visibility.Collapsed;
                }

                if (analogMeasurements.Count > 0)
                {
                    foreach (AnalogMeasurement measurement in analogMeasurements)
                    {
                        AnalogMeasurementControl analogMeasurementControl = new AnalogMeasurementControl()
                        {
                            DataContext = measurement
                        };

                        propertiesModelView.AnalogControls.Add(analogMeasurementControl);
                    }


                    propertiesModelView.AnalogMeasurementVisibility = Visibility.Visible;
                }
                else
                {
                    propertiesModelView.AnalogMeasurementVisibility = Visibility.Collapsed;
                }
            }

            ShellFillerShell sfs = new ShellFillerShell() { /*DataContext = this*/ };
            sfs.Header.Text = "Properties";
            sfs.MainScroll.Content = propertiesControl;
            propertiesControl.DataContext = propertiesModelView;

            if (!propertiesModelView.IsOpen)
            {
                propertiesModelView.IsOpen = true;

                new MainShellViewModel().PlaceOrFocusControlInShell(PropertiesModelView.Position, sfs, false, "Properties");

                return;
            }

            // JAKO lose !!!!!!!!!!!!!!!
            new MainShellViewModel().PlaceOrFocusControlInShell(PropertiesModelView.Position, sfs, true, "Properties");
        }
    }
}

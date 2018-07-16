using DispatcherApp.Model;
using DispatcherApp.Model.DrawingAlgorithms.Schematic.RadialNetwork;
using DispatcherApp.Model.Properties.DMSProperties;
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

            foreach (ElementProperties property in properties.Values)
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
    }
}

using DNP3ConfigParser.Configuration;
using DNP3ConfigParser.Configuration.DNP3DeviceProfileJan2010ConfigModel;
using DNP3ConfigParser.Configurations;
using DNP3ConfigParser.Enums.DeviceConfig;
using DNP3ConfigParser.Enums.NetworkConfig;
using DNP3DataPointsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DNP3ConfigParser.Parsers
{
    public class DNP3DeviceProfileJan2010Parser : UniversalConfigurationParser
    {
        public CompleteConfiguration Configuration { get; set; }

        public DNP3DeviceProfileJan2010Parser(XDocument configFile)
        {
            this.configFile = configFile;
            Configuration = new CompleteConfiguration();
        }

        public override void Parse()
        {
            ParseDeviceConfiguration();
            ParseSerialConfiguration();
            ParseNetworkConfiguration();
            ParseDataPointsListConfiguration();
        }

        private void ParseDeviceConfiguration()
        {
            XElement root = configFile.Root;
            XNamespace defaultNamespace = root.GetDefaultNamespace();

            IEnumerable<XElement> elements = root.Descendants(defaultNamespace + "deviceConfig");

            if (elements.Count() > 0)
            {
                XElement element = elements.ElementAt(0).Element(defaultNamespace + "deviceFunction");

                if (element != null)
                {
                    XElement temp1;
                    if ((temp1 = element.Element(defaultNamespace + "capabilities")) != null)
                    {
                        XElement temp2;
                        if ((temp2 = temp1.Element(defaultNamespace + "outstation")) != null)
                        {
                            Configuration.DeviceConfiguration.DeviceFunctionCapabilities.Add(DeviceFunction.OUTSTATION);
                        }

                        temp2 = temp1;
                        if ((temp2 = temp1.Element(defaultNamespace + "master")) != null)
                        {
                            Configuration.DeviceConfiguration.DeviceFunctionCapabilities.Add(DeviceFunction.MASTER);
                        }
                    }

                    temp1 = element;
                    if ((temp1 = element.Element(defaultNamespace + "currentValue")) != null)
                    {
                        XElement temp2;
                        if ((temp2 = temp1.Element(defaultNamespace + "outstation")) != null)
                        {
                            Configuration.DeviceConfiguration.DeviceFunctionCurrentValue = DeviceFunction.OUTSTATION;
                        }

                        temp2 = temp1;
                        if ((temp2 = temp1.Element(defaultNamespace + "master")) != null)
                        {
                            Configuration.DeviceConfiguration.DeviceFunctionCurrentValue = DeviceFunction.MASTER;
                        }
                    }
                }

                element = elements.ElementAt(0).Element(defaultNamespace + "vendorName");

                if (element != null)
                {
                }

                element = elements.ElementAt(0).Element(defaultNamespace + "deviceName");

                if (element != null)
                {
                    if (element != null)
                    {
                        XElement temp1;
                        if ((temp1 = element.Element(defaultNamespace + "currentValue")) != null)
                        {
                            Configuration.DeviceConfiguration.DeviceName = temp1.Value;
                        }
                    }
                }

                element = elements.ElementAt(0).Element(defaultNamespace + "hardwareVersion");

                if (element != null)
                {
                }

                element = elements.ElementAt(0).Element(defaultNamespace + "softwareVersion");

                if (element != null)
                {
                }

                element = elements.ElementAt(0).Element(defaultNamespace + "documentVersionNumber");

                if (element != null)
                {
                }
            }
        }

        private void ParseSerialConfiguration()
        {
        }

        private void ParseNetworkConfiguration()
        {
            XElement root = configFile.Root;
            XNamespace defaultNamespace = root.GetDefaultNamespace();

            IEnumerable<XElement> elements = root.Descendants(defaultNamespace + "networkConfig");

            if (elements.Count() > 0)
            {
                XElement element = elements.ElementAt(0).Element(defaultNamespace + "portName");

                if (element != null)
                {
                    XElement temp1;
                    if ((temp1 = element.Element(defaultNamespace + "currentValue")) != null)
                    {
                        Configuration.NetworkConfiguration.PortName = temp1.Value;
                    }
                }

                element = elements.ElementAt(0).Element(defaultNamespace + "typeOfEndPoint");

                if (element != null)
                {
                    XElement temp1;
                    if ((temp1 = element.Element(defaultNamespace + "capabilities")) != null)
                    {
                        XElement temp2;
                        if ((temp2 = temp1.Element(defaultNamespace + "tcpListening")) != null)
                        {
                            Configuration.NetworkConfiguration.TypeOfEndpointCapabilities.Add(TypeOfEndpoint.TCP_Listening);
                        }
                    }

                    temp1 = element;
                    if ((temp1 = element.Element(defaultNamespace + "currentValue")) != null)
                    {
                        XElement temp2;
                        if ((temp2 = temp1.Element(defaultNamespace + "tcpListening")) != null)
                        {
                            Configuration.NetworkConfiguration.TypeOfEndpointCurrentValue = (TypeOfEndpoint.TCP_Listening);
                        }
                    }
                }

                element = elements.ElementAt(0).Element(defaultNamespace + "ipAddress");

                if (element != null)
                {
                    XElement temp1;
                    if ((temp1 = element.Element(defaultNamespace + "value")) != null)
                    {
                        Configuration.NetworkConfiguration.IpAddress = temp1.Value;
                    }
                }

                element = elements.ElementAt(0).Element(defaultNamespace + "tcpConnectionEstablishment");

                element = elements.ElementAt(0).Element(defaultNamespace + "ipAddressOfRemoteDevice");

                if (element != null)
                {
                    XElement temp1;
                    if ((temp1 = element.Element(defaultNamespace + "currentValue")) != null)
                    {
                        foreach (XElement xElement in temp1.Elements(defaultNamespace + "address"))
                        {
                            Configuration.NetworkConfiguration.RemoteIPAddress = xElement.Value;
                        }
                    }
                }

                element = elements.ElementAt(0).Element(defaultNamespace + "tcpListenPort");

                if (element != null)
                {
                    XElement temp1;
                    if ((temp1 = element.Element(defaultNamespace + "capabilities")) != null)
                    {
                        XElement temp2;
                        if ((temp2 = temp1.Element(defaultNamespace + "fixedAt20000")) != null)
                        {
                            Configuration.NetworkConfiguration.TCPListenPortRemoteCapabilities.Add(TCPListenPortNumber.FIXED_AT_20000);
                        }
                    }

                    temp1 = element;
                    if ((temp1 = element.Element(defaultNamespace + "currentValue")) != null)
                    {
                        XElement temp2;
                        if ((temp2 = temp1.Element(defaultNamespace + "fixedAt20000")) != null)
                        {
                            //Configuration.DeviceConfiguration.DeviceFunctionCurrentValue = DeviceFunction.OUTSTATION;
                        }
                    }
                }

                element = elements.ElementAt(0).Element(defaultNamespace + "tcpPortOfRemoteDevice");

                if (element != null)
                {
                    XElement temp1;
                    if ((temp1 = element.Element(defaultNamespace + "capabilities")) != null)
                    {
                        XElement temp2;
                        if ((temp2 = temp1.Element(defaultNamespace + "fixedAt20000")) != null)
                        {
                            Configuration.NetworkConfiguration.TCPListenPortRemoteCapabilities.Add(TCPListenPortNumber.FIXED_AT_20000);
                        }
                    }

                    temp1 = element;
                    if ((temp1 = element.Element(defaultNamespace + "currentValue")) != null)
                    {
                        XElement temp2;
                        if ((temp2 = temp1.Element(defaultNamespace + "fixedAt20000")) != null)
                        {
                            //Configuration.NetworkConfiguration.TCPListenPortCapabilities.Add(TCPListenPortNumber.FIXED_AT_20000);
                        }
                    }
                }
            }
        }

        private void ParseDataPointsListConfiguration()
        {
            XElement root = configFile.Root;
            XNamespace defaultNamespace = root.GetDefaultNamespace();

            IEnumerable<XElement> elements = root.Descendants(defaultNamespace + "dataPointsList");

            if (elements.Count() > 0)
            {
                XElement element = elements.ElementAt(0).Element(defaultNamespace + "analogInputPoints");

                if (element != null)
                {
                    XElement temp1;
                    if ((temp1 = element.Element(defaultNamespace + "dataPoints")) != null)
                    {
                        foreach (XElement temp2 in temp1.Elements(defaultNamespace + "analogInput"))
                        {
                            AnalogInputPoint analogInputPoint = new AnalogInputPoint();

                            XElement temp3;
                            if ((temp3 = temp2.Element(defaultNamespace + "index")) != null)
                            {
                                analogInputPoint.Index = int.Parse(temp3.Value);
                            }
                            if ((temp3 = temp2.Element(defaultNamespace + "name")) != null)
                            {
                                analogInputPoint.Name = temp3.Value;
                            }
                            if ((temp3 = temp2.Element(defaultNamespace + "changeEventClass")) != null)
                            {
                                analogInputPoint.ChangeEventClass = int.Parse(temp3.Value);
                            }
                            if ((temp3 = temp2.Element(defaultNamespace + "minIntegerTransmittedValue")) != null)
                            {
                                analogInputPoint.MinIntegerTransmittedValue = int.Parse(temp3.Value);
                            }
                            if ((temp3 = temp2.Element(defaultNamespace + "maxIntegerTransmittedValue")) != null)
                            {
                                analogInputPoint.MaxIntegerTransmittedValue = int.Parse(temp3.Value);
                            }
                            if ((temp3 = temp2.Element(defaultNamespace + "scaleFactor")) != null)
                            {
                                analogInputPoint.ScaleFactor = int.Parse(temp3.Value);
                            }
                            if ((temp3 = temp2.Element(defaultNamespace + "scaleOffset")) != null)
                            {
                                analogInputPoint.ScaleOffset = double.Parse(temp3.Value);
                            }
                            if ((temp3 = temp2.Element(defaultNamespace + "units")) != null)
                            {
                                analogInputPoint.Units = temp3.Value;
                            }
                            if ((temp3 = temp2.Element(defaultNamespace + "resolution")) != null)
                            {
                                analogInputPoint.Resolution = int.Parse(temp3.Value);
                            }
                            if ((temp3 = temp2.Element(defaultNamespace + "description")) != null)
                            {
                                analogInputPoint.Description = temp3.Value;
                            }

                            Configuration.DataPointsListConfiguration.AnalogInputPoints.Add(analogInputPoint);
                        }
                    }
                }
            }
        }
    }
}

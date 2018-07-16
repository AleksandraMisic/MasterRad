using DispatcherApp.View;
using DispatcherApp;
using FTN.Common;
using PubSubscribe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows;
using DMSCommon.Model;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.IO;
using System.Threading;
using DispatcherApp.Model.Properties;
using System.ServiceModel;
using System.Windows.Data;
using IMSContract;
using DispatcherApp.View.CustomControls;
using DispatcherApp.View.CustomControls.NetworkElementsControls;
using DispatcherApp.Model.Measurements;
using OMSSCADACommon;
using System.Threading.Tasks;
using DispatcherApp.View.CustomControls.TabContentControls;
using GravityAppsMandelkowMetroCharts;
using DMSCommon;
using DispatcherApp.ViewModel.ShellFillerModelViews;
using UIShell.ViewModel;
using UIShell.Model;
using UIShell.View;
using TransactionManagerContract.ClientDMS;
using TransactionManagerContract.ClientIMS;
using TransactionManagerContract;

namespace DispatcherApp.ViewModel
{
    public class MainShellViewModel : AbstractMainShellViewModel
    {
        public Task blinkTask;

        public CancellationTokenSource tokenSource = new CancellationTokenSource();

        private ClientDMSProxy dMSProxy;
        private ClientIMSProxy iMSProxy;

        private FrameworkElement frameworkElement = new FrameworkElement();

        private Dictionary<string, bool> isSourceOpen = new Dictionary<string, bool>();

        private ObservableCollection<ChartSeries> chartSeries = new ObservableCollection<ChartSeries>();
        private ObservableCollection<UIElement> chartBorderItems = new ObservableCollection<UIElement>();
        private string chartTitle = "";
        private string chartSubtitle = "";

        private System.Timers.Timer crewTimer = new System.Timers.Timer();
        private double timerValue = 0;
        private double investigationMax = 4;
        private double maxValue = 4;
        private double minValue = 0;
        
        private Subscriber subscriber;

        #region Bindings
        private Dictionary<long, Element> Network = new Dictionary<long, Element>();
        private Dictionary<long, string> Sources = new Dictionary<long, string>();

        private ObservableCollection<Element> breakers = new ObservableCollection<Element>();

        private Dictionary<long, ElementProperties> properties = new Dictionary<long, ElementProperties>();
        private Dictionary<long, ResourceDescription> resourceProperties = new Dictionary<long, ResourceDescription>();
        private ElementProperties currentProperty = new ElementProperties();
        private long currentPropertyMRID;

        private int commandsIndex = 0;
        private bool test = true;

        private ObservableCollection<IncidentReport> incidentReports = new ObservableCollection<IncidentReport>();
        private ObservableCollection<Crew> crews = new ObservableCollection<Crew>();

        private Dictionary<long, Measurement> measurements = new Dictionary<long, Measurement>();

        private Dictionary<long, ObservableCollection<UIElement>> uiNetworks = new Dictionary<long, ObservableCollection<UIElement>>();
        private ObservableCollection<UIElement> mainCanvases = new ObservableCollection<UIElement>();
        private Dictionary<long, int> networkDepth = new Dictionary<long, int>();
        private Canvas mainCanvas = new Canvas();
        private double startHeight = 20;
        private double startWidth = 3;
        private double currentHeight = 20;
        private double currentWidth = 3;

        private ObservableCollection<TreeViewItem> networkMapsBySource = new ObservableCollection<TreeViewItem>();
        private ObservableCollection<Button> networkMapsBySourceButton = new ObservableCollection<Button>();
        private Dictionary<long, NetworkModelControlExtended> networModelControls = new Dictionary<long, NetworkModelControlExtended>();
        #endregion

        #region Commands
        private RelayCommand openNetworkExplorerCommand;
        private RelayCommand openIncidentExplorerCommand;
        private RelayCommand openReportExplorerCommand;
        private RelayCommand openPropertiesCommand;
        private RelayCommand openOutputCommand;
        private RelayCommand openNetworkViewCommand;

        private RelayCommand _closeControlCommand;

        private RelayCommand _propertiesCommand;

        private RelayCommand _sendCrewCommand;

        private RelayCommand _executeSwitchCommand;

        private RelayCommand _generateIncidentByDateChartCommand;
        private RelayCommand _generateIncidentByBreakerChartCommand;
        private RelayCommand _generateStatesByBreakerChartCommand;
        #endregion

        #region Constructor
        public MainShellViewModel()
        {
            TopMenu = new ObservableCollection<UserControl>();
            TopMenu.Add(new TopMenu());

            subscriber = new Subscriber();
            subscriber.Subscribe();
            subscriber.publishDigitalUpdateEvent += GetDigitalUpdate;
            subscriber.publishAnalogUpdateEvent += GetAnalogUpdate;
            subscriber.publishCrewEvent += GetCrewUpdate;
            subscriber.publishIncident += GetIncident;
            subscriber.publishCall += GetCallFromConsumers;
            subscriber.publiesBreakers += SearchForIncident;

            dMSProxy = new ClientDMSProxy();
            
            try
            {
                List<Source> sourcesList = dMSProxy.GetAllSources();
            }
            catch(Exception e) { }

            try
            {
                List<IncidentReport> reportsList = iMSProxy.GetAllReports();
            }
            catch (Exception e) { }
        }

        public void InitElementsAndProperties(TMSAnswerToClient answerFromTransactionManager)
        {
            if (answerFromTransactionManager != null && answerFromTransactionManager.Elements != null && answerFromTransactionManager.ResourceDescriptions != null)
            {
                foreach (Element element in answerFromTransactionManager.Elements)
                {
                    this.Network.Add(element.ElementGID, element);
                }

                foreach (ResourceDescription rd in answerFromTransactionManager.ResourceDescriptions)
                {
                    Element element = null;
                    this.Network.TryGetValue(rd.GetProperty(ModelCode.IDOBJ_GID).AsLong(), out element);

                    if (element != null)
                    {
                        if (element is Source)
                        {
                            this.Sources.Add(element.ElementGID, element.MRID);
                            EnergySourceProperties properties = new EnergySourceProperties() { IsEnergized = element.IsEnergized, IsUnderScada = element.UnderSCADA };
                            properties.ReadFromResourceDescription(rd);
                            this.properties.Add(element.ElementGID, properties);
                        }
                        else if (element is Consumer)
                        {
                            EnergyConsumerProperties properties = new EnergyConsumerProperties() { IsEnergized = element.IsEnergized, IsUnderScada = element.UnderSCADA };
                            properties.ReadFromResourceDescription(rd);
                            this.properties.Add(element.ElementGID, properties);
                        }
                        else if (element is ACLine)
                        {
                            ACLineSegmentProperties properties = new ACLineSegmentProperties() { IsEnergized = element.IsEnergized, IsUnderScada = element.UnderSCADA };
                            properties.ReadFromResourceDescription(rd);
                            this.properties.Add(element.ElementGID, properties);
                        }
                        else if (element is Node)
                        {
                            ConnectivityNodeProperties properties = new ConnectivityNodeProperties() { IsEnergized = element.IsEnergized, IsUnderScada = element.UnderSCADA };
                            properties.ReadFromResourceDescription(rd);
                            this.properties.Add(element.ElementGID, properties);
                        }
                        else if (element is Switch)
                        {
                            Switch breaker = element as Switch;
                            this.Breakers.Add(element);
                            BreakerProperties properties = new BreakerProperties() { IsEnergized = element.IsEnergized, IsUnderScada = element.UnderSCADA, Incident = element.Incident, CanCommand = breaker.CanCommand };

                            if (breaker.State == SwitchState.Open)
                            {
                                properties.State = OMSSCADACommon.States.OPEN;
                            }
                            else if (breaker.State == SwitchState.Closed)
                            {
                                properties.State = OMSSCADACommon.States.CLOSED;
                            }

                            properties.ValidCommands.Add(CommandTypes.CLOSE);
                            this.CommandIndex = 0;

                            properties.ReadFromResourceDescription(rd);
                            this.properties.Add(element.ElementGID, properties);
                        }
                    }
                }

                foreach (ElementProperties properties in this.properties.Values)
                {
                    Element element = null;
                    this.Network.TryGetValue(properties.GID, out element);

                    if (element != null && element is Branch)
                    {
                        Element node = null;
                        Branch currentBranch = (Branch)element;
                        this.Network.TryGetValue(currentBranch.End1, out node);

                        if (node != null)
                        {
                            Element branch = null;
                            Node parent = (Node)node;
                            this.Network.TryGetValue(parent.Parent, out branch);

                            if (branch != null)
                            {
                                ElementProperties parentProperties = null;
                                this.properties.TryGetValue(branch.ElementGID, out parentProperties);

                                if (parentProperties != null)
                                {
                                    properties.Parent = parentProperties;
                                }
                            }
                        }
                    }
                }
            }

            foreach (ResourceDescription rd in answerFromTransactionManager.ResourceDescriptionsOfMeasurment)
            {
                ResourceDescription meas;
                try
                {
                    meas = answerFromTransactionManager.ResourceDescriptions.Where(p => p.GetProperty(ModelCode.IDOBJ_MRID).AsString() == rd.GetProperty(ModelCode.IDOBJ_MRID).AsString()).FirstOrDefault();
                }
                catch { continue; }

                if (meas != null)
                {
                    try
                    {
                        long psr = meas.GetProperty(ModelCode.MEASUREMENT_PSR).AsLong();
                        DMSType type = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(psr);

                        if (type == DMSType.BREAKER)
                        {
                            meas.UpdateProperty(rd.GetProperty(ModelCode.DISCRETE_NORMVAL));
                            DigitalMeasurement measurement = new DigitalMeasurement();
                            measurement.ReadFromResourceDescription(meas);

                            Element element = null;
                            Network.TryGetValue(psr, out element);

                            Switch breaker = null;
                            if (element != null)
                            {
                                breaker = element as Switch;

                                if (breaker != null)
                                {
                                    if (breaker.State == SwitchState.Open)
                                    {
                                        measurement.State = OMSSCADACommon.States.OPEN;
                                    }
                                    else if (breaker.State == SwitchState.Closed)
                                    {
                                        measurement.State = OMSSCADACommon.States.CLOSED;
                                    }
                                }
                            }

                            ElementProperties properties;
                            Properties.TryGetValue(psr, out properties);

                            if (properties != null)
                            {
                                properties.Measurements.Add(measurement);
                            }

                            this.Measurements.Add(measurement.GID, measurement);
                        }
                        else if (type == DMSType.ENERGCONSUMER)
                        {
                            meas.UpdateProperty(rd.GetProperty(ModelCode.ANALOG_NORMVAL));
                            AnalogMeasurement measurement = new AnalogMeasurement();
                            measurement.ReadFromResourceDescription(meas);

                            ElementProperties properties;
                            Properties.TryGetValue(psr, out properties);

                            if (properties != null)
                            {
                                properties.Measurements.Add(measurement);
                            }

                            properties.IsUnderScada = true;
                            this.Measurements.Add(measurement.GID, measurement);
                        }
                    }
                    catch { }
                }
            }

            if (answerFromTransactionManager.Crews != null)
            {
                foreach (Crew crew in answerFromTransactionManager.Crews)
                {
                    this.Crews.Add(crew);
                }
            }

            if (answerFromTransactionManager.IncidentReports != null)
            {
                foreach (IncidentReport incident in answerFromTransactionManager.IncidentReports)
                {
                    this.IncidentReports.Insert(0, incident);
                }
            }
        }
        
        #endregion

        #region DrawGraph

        private void PlaceSource(long id, string mrid)
        {
            Button sourceButton = new Button() { Width = 18, Height = 18 };
            sourceButton.Background = Brushes.Transparent;
            sourceButton.BorderThickness = new Thickness(0);
            sourceButton.BorderBrush = Brushes.Transparent;
            sourceButton.ToolTip = mrid;
            sourceButton.Content = new Image() { Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "/../../View/Resources/Images/triangle.png")) };
            Canvas.SetLeft(sourceButton, mainCanvas.Width / 2 - sourceButton.Width / 2);
            Canvas.SetZIndex(sourceButton, 5);
            mainCanvas.Children.Add(sourceButton);

            SetProperties(sourceButton, id);
        }

        private void SetProperties(Button button, long id)
        {
            ElementProperties property;
            Element element;
            this.properties.TryGetValue(id, out property);
            this.Network.TryGetValue(id, out element);

            button.Command = PropertiesCommand;
            button.CommandParameter = id;
        }
        #endregion

        #region Command execution
        public RelayCommand GenerateIncidentByDateChartCommand
        {
            get
            {
                return _generateIncidentByDateChartCommand ?? new RelayCommand(
                    (parameter) =>
                    {
                        ExecuteGenerateIncidentByDateChartCommand(parameter);
                    });
            }
        }

        public RelayCommand GenerateIncidentByBreakerChartCommand
        {
            get
            {
                return _generateIncidentByBreakerChartCommand ?? new RelayCommand(
                    (parameter) =>
                    {
                        ExecuteGenerateIncidentByBreakerChartCommand(parameter);
                    });
            }
        }

        public RelayCommand GenerateStatesByBreakerChartCommand
        {
            get
            {
                return _generateStatesByBreakerChartCommand ?? new RelayCommand(
                    (parameter) =>
                    {
                        ExecuteGenerateStatesByBreakerChartCommand(parameter);
                    });
            }
        }

        public RelayCommand OpenNetworkExplorerCommand
        {
            get
            {
                return openNetworkExplorerCommand ?? new RelayCommand(
                    (parameter) =>
                    {
                        ExecuteOpenNetworkExplorerCommand();
                    });
            }
        }

        public RelayCommand OpenIncidentExplorerCommand
        {
            get
            {
                return openIncidentExplorerCommand ?? new RelayCommand(
                    (parameter) =>
                    {
                        ExecuteOpenIncidentExplorerCommand();
                    });
            }
        }

        public RelayCommand OpenReportExplorerCommand
        {
            get
            {
                return openReportExplorerCommand ?? new RelayCommand(
                    (parameter) =>
                    {
                        ExecuteOpenReportExplorerCommand();
                    });
            }
        }

        public RelayCommand OpenPropertiesCommand
        {
            get
            {
                return openPropertiesCommand ?? new RelayCommand(
                    (parameter) =>
                    {
                        ExecuteOpenPropertiesCommand();
                    });
            }
        }

        public RelayCommand OpenOutputCommand
        {
            get
            {
                return openOutputCommand ?? new RelayCommand(
                    (parameter) =>
                    {
                        ExecuteOpenOutputCommand();
                    });
            }
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

        public RelayCommand CloseControlCommand
        {
            get
            {
                return _closeControlCommand ?? new RelayCommand(
                    (parameter) =>
                    {
                        ExecuteCloseControlCommand(parameter);
                    });
            }
        }

        public RelayCommand PropertiesCommand
        {
            get
            {
                return _propertiesCommand ?? new RelayCommand(
                    (parameter) =>
                    {
                        ExecutePropertiesCommand(parameter);
                    });
            }
        }

        public RelayCommand SendCrewCommand
        {
            get
            {
                return _sendCrewCommand ?? new RelayCommand(
                    (parameter) =>
                    {
                        ExecuteSendCrewCommand(parameter);
                    });
            }
        }

        public RelayCommand ExecuteSwitchCommand
        {
            get
            {
                return _executeSwitchCommand ?? new RelayCommand(
                    (parameter) =>
                    {
                        ExecuteSwitchCommandd(parameter);
                    });
            }
        }

        private void ExecuteGenerateIncidentByDateChartCommand(object parameter)
        {
            try
            {
                bool allDates = false;
                DateTime date;
                try
                {
                    date = (DateTime)parameter;
                }
                catch
                {
                    allDates = true;
                    date = DateTime.UtcNow;
                }

                List<List<IncidentReport>> reportsByBreaker = new List<List<IncidentReport>>();
                List<string> mrids = new List<string>();

                foreach (Switch breaker in this.Breakers)
                {
                    mrids.Add(breaker.MRID);
                }

                if (!allDates)
                {
                    //reportsByBreaker = ProxyToOMS.GetReportsForSpecificDateSortByBreaker(mrids, date);
                }
                else
                {
                    //reportsByBreaker = ProxyToOMS.GetAllReportsSortByBreaker(mrids);
                }

                ClusteredColumnChart chart = new ClusteredColumnChart();
                this.ChartSeries.Clear();
                this.ChartBorderItems.Clear();

                int i = 0;
                foreach (List<IncidentReport> reports in reportsByBreaker)
                {
                    if (reports.Count == 0)
                    {
                        reports.Add(new IncidentReport() { LostPower = 0, MrID = "a" });
                    }
                    else
                    {
                        foreach (IncidentReport report in reports)
                        {
                            report.LostPower = reports.Count;
                        }
                    }

                    ChartSeries series = new ChartSeries();
                    series.SeriesTitle = mrids[i++];
                    series.ItemsSource = reports;
                    series.DisplayMember = "MrID";
                    series.ValueMember = "LostPower";

                    this.ChartSeries.Add(series);
                }

                this.ChartTitle = "Number of Incidents for Breakers";
                if (!allDates)
                {
                    this.ChartSubtitle = "Date: " + date.Day + "/" + date.Month + "/" + date.Year;
                }
                else
                {
                    this.ChartSubtitle = "All days";
                }

                chart.Series = this.ChartSeries;
                chart.HorizontalAlignment = HorizontalAlignment.Stretch;
                chart.VerticalAlignment = VerticalAlignment.Stretch;
                chart.MinHeight = 400;
                this.ChartBorderItems.Add(chart);
            }
            catch { }
        }

        private void ExecuteGenerateIncidentByBreakerChartCommand(object parameter)
        {
            try
            {
                Switch breaker;
                try
                {
                    breaker = (Switch)parameter;
                }
                catch
                {
                    return;
                }

                List<List<IncidentReport>> reportsByBreaker = new List<List<IncidentReport>>();

                //reportsByBreaker = ProxyToOMS.GetReportsForMrID(breaker.MRID);

                ClusteredColumnChart chart = new ClusteredColumnChart();
                this.ChartSeries.Clear();
                this.ChartBorderItems.Clear();

                int i = 0;
                foreach (List<IncidentReport> reports in reportsByBreaker)
                {
                    if (reports.Count == 0)
                    {
                        reports.Add(new IncidentReport() { LostPower = 0, MrID = "a" });
                    }
                    else
                    {
                        foreach (IncidentReport report in reports)
                        {
                            report.LostPower = reports.Count;
                        }
                    }

                    ChartSeries series = new ChartSeries();
                    series.SeriesTitle = reports[0].Time.Day + "/" + reports[0].Time.Month + "/" + reports[0].Time.Year;
                    series.ItemsSource = reports;
                    series.DisplayMember = "MrID";
                    series.ValueMember = "LostPower";

                    this.ChartSeries.Add(series);
                }

                this.ChartTitle = "Number of Incidents by Days";
                this.ChartSubtitle = "Breaker: " + breaker.MRID;

                chart.Series = this.ChartSeries;
                chart.HorizontalAlignment = HorizontalAlignment.Stretch;
                chart.VerticalAlignment = VerticalAlignment.Stretch;
                chart.MinHeight = 400;
                this.ChartBorderItems.Add(chart);
            }
            catch { }
        }

        private void ExecuteGenerateStatesByBreakerChartCommand(object parameter)
        {
            try
            {
                Switch breaker;
                try
                {
                    breaker = (Switch)parameter;
                }
                catch
                {
                    return;
                }

                List<List<SwitchStateReport>> reportsByBreaker = new List<List<SwitchStateReport>>();

                try
                {
                    //reportsByBreaker = ProxyToOMS.GetElementStateReportsForMrID(breaker.MRID);
                }
                catch { }

                ClusteredColumnChart chart = new ClusteredColumnChart();
                this.ChartSeries.Clear();
                this.ChartBorderItems.Clear();

                int i = 0;
                foreach (List<SwitchStateReport> reports in reportsByBreaker)
                {
                    ChartSeries series = new ChartSeries();
                    series.SeriesTitle = string.Format("{0}/{1}/{2}\n{3}:{4}:{5}", reports[0].Time.Day, reports[0].Time.Month, reports[0].Time.Year, reports[0].Time.Hour, reports[0].Time.Minute, reports[0].Time.Second);
                    series.ItemsSource = reports;
                    series.DisplayMember = "MrID";
                    series.ValueMember = "State";

                    this.ChartSeries.Add(series);
                }

                this.ChartTitle = "States of a Breaker (0 - Closed, 1 - Opened)";
                this.ChartSubtitle = "Breaker: " + breaker.MRID;

                chart.Series = this.ChartSeries;
                chart.HorizontalAlignment = HorizontalAlignment.Stretch;
                chart.VerticalAlignment = VerticalAlignment.Stretch;
                chart.MinHeight = 400;
                this.ChartBorderItems.Add(chart);
            }
            catch { }
        }

        private void ExecuteSwitchCommandd(object parameter)
        {
            Measurement measurement = this.Measurements.Where(m => m.Value.MRID == (string)parameter).FirstOrDefault().Value;
            if (measurement != null)
            {
                ElementProperties elementProperties;
                Properties.TryGetValue(measurement.Psr, out elementProperties);

                if (elementProperties != null)
                {
                    elementProperties.CanCommand = false;
                }
            }

            try
            {
                //ProxyToOMS.SendCommandToSCADA(TypeOfSCADACommand.WriteDigital, (string)parameter, CommandTypes.CLOSE, 0);
            }
            catch { }
        }

        private void ExecuteSendCrewCommand(object parameter)
        {
            var values = (object[])parameter;
            var datetime = (DateTime)values[0];
            var crew = (Crew)values[1];

            if (crew == null)
            {
                return;
            }

            IncidentReport report = new IncidentReport();
            foreach (IncidentReport ir in IncidentReports)
            {
                if (DateTime.Compare(ir.Time, (DateTime)datetime) == 0)
                {
                    report = ir;
                    break;
                }
            }

            //report.CrewSent = true;

            //crew.Working = true;
            RaisePropertyChanged("Crews");

            if (report.IncidentState == IncidentState.UNRESOLVED)
            {
                //report.InvestigationCrew = crew;
                //report.CurrentValue = 0;
                //report.MaxValue = investigationMax;

                //tokenSource = new CancellationTokenSource();
                //CancellationToken token = tokenSource.Token;
                //Task.Factory.StartNew(() => ProgressBarChange(report, token), token);

                //report.IncidentState = IncidentState.INVESTIGATING;
            }
            else if (report.IncidentState == IncidentState.READY_FOR_REPAIR)
            {
                //report.RepairCrew = crew;
                //report.CurrentValue = 0;
                //report.MaxValue = report.RepairTime.TotalMinutes/10;

                //tokenSource = new CancellationTokenSource();
                //CancellationToken token = tokenSource.Token;
                //Task.Factory.StartNew(() => ProgressBarChange(report, token), token);

                //report.IncidentState = IncidentState.REPAIRING;
            }

            //ProxyToOMS.SendCrew(report, crew);

            //try
            //{
            //    ElementProperties element = Properties.Where(p => p.Value.MRID == report.MrID).FirstOrDefault().Value;
            //    element.CrewSent = true;
            //}
            //catch {  }
        }

        private void ExecutePropertiesCommand(object parameter)
        {
            //Element element;
            //properties.TryGetValue((long)parameter, out currentProperty);
            //Network.TryGetValue((long)parameter, out element);

            //if (currentProperty != null)
            //{
            //    CurrentPropertyMRID = currentProperty.GID;
            //    bool exists = false;
            //    int i = 0;

            //    for (i = 0; i < RightTabControlTabs.Count; i++)
            //    {
            //        if (RightTabControlTabs[i].Header as string == "Properties")
            //        {
            //            SetTabContent(RightTabControlTabs[i], element);
            //            exists = true;
            //            this.RightTabControlIndex = i;
            //            break;
            //        }
            //    }

            //    if (!exists)
            //    {
            //        BorderTabItem ti = new BorderTabItem() { Header = "Properties", Style = (Style)frameworkElement.FindResource("TabItemRightStyle") };
            //        ti.Title.Text = "Properties";
            //        SetTabContent(ti, element);
            //        //if (!RightTabControlTabs.Contains(ti))
            //        //{
            //        this.RightTabControlTabs.Add(ti);
            //        this.RightTabControlIndex = this.RightTabControlTabs.Count - 1;
            //        //}
            //    }

            //    this.RightTabControlVisibility = Visibility.Visible;
            //}
        }

        #endregion

        #region Properties
        public double TimerValue
        {
            get
            {
                return timerValue;
            }
            set
            {
                timerValue = value;
                RaisePropertyChanged("TimerValue");
            }
        }

        public double MaxValue
        {
            get
            {
                return maxValue;
            }
            set
            {
                maxValue = value;
                RaisePropertyChanged("MaxValue");
            }
        }

        public double MinValue
        {
            get
            {
                return minValue;
            }
            set
            {
                minValue = value;
                RaisePropertyChanged("MinValue");
            }
        }

        public int CommandIndex
        {
            get
            {
                return commandsIndex;
            }
            set
            {
                commandsIndex = value;
                RaisePropertyChanged("CommandIndex");
            }
        }

        public ObservableCollection<IncidentReport> IncidentReports
        {
            get
            {
                return incidentReports;
            }
            set
            {
                incidentReports = value;
            }
        }

        public ObservableCollection<Crew> Crews
        {
            get
            {
                return crews;
            }
            set
            {
                crews = value;
            }
        }

        public ObservableCollection<TreeViewItem> NetworkMapsBySource
        {
            get
            {
                return networkMapsBySource;
            }
            set
            {
                networkMapsBySource = value;
            }
        }

        public ObservableCollection<UIElement> MainCanvases
        {
            get
            {
                return mainCanvases;
            }
            set
            {
                mainCanvases = value;
            }
        }

        public ObservableCollection<Button> NetworkMapsBySourceButton
        {
            get
            {
                return networkMapsBySourceButton;
            }
            set
            {
                networkMapsBySourceButton = value;
            }
        }

        public ObservableCollection<ChartSeries> ChartSeries
        {
            get
            {
                return chartSeries;
            }
            set
            {
                chartSeries = value;
            }
        }

        public ObservableCollection<UIElement> ChartBorderItems
        {
            get
            {
                return chartBorderItems;
            }
            set
            {
                chartBorderItems = value;
            }
        }

        public ObservableCollection<Element> Breakers
        {
            get
            {
                return breakers;
            }
            set
            {
                breakers = value;
            }
        }

        public Dictionary<long, ObservableCollection<UIElement>> UINetworks
        {
            get
            {
                return uiNetworks;
            }
            set
            {
                uiNetworks = value;
            }
        }

        public Dictionary<long, ElementProperties> Properties
        {
            get
            {
                return properties;
            }
            set
            {
                properties = value;
                RaisePropertyChanged("Properties");
            }
        }

        public ElementProperties CurrentProperty
        {
            get
            {
                return currentProperty;
            }
            set
            {
                currentProperty = value;
                RaisePropertyChanged("CurrentProperty");
            }
        }

        public long CurrentPropertyMRID
        {
            get
            {
                return currentPropertyMRID;
            }
            set
            {
                currentPropertyMRID = value;
                RaisePropertyChanged("CurrentPropertyMRID");
            }
        }

        public Dictionary<long, Measurement> Measurements
        {
            get
            {
                return measurements;
            }
            set
            {
                measurements = value;
                RaisePropertyChanged("Measurements");
            }
        }

        public string ChartTitle
        {
            get
            {
                return chartTitle;
            }
            set
            {
                chartTitle = value;
                RaisePropertyChanged("ChartTitle");
            }
        }

        public string ChartSubtitle
        {
            get
            {
                return chartSubtitle;
            }
            set
            {
                chartSubtitle = value;
                RaisePropertyChanged("ChartSubtitle");
            }
        }

        #endregion Properties

        #region Publish methods
        private void GetDigitalUpdate(List<UIUpdateModel> update)
        {
            if (update != null)
            {
                if (update.ElementAt(0).IsElementAdded == true)
                {
                    NetTcpBinding binding = new NetTcpBinding();
                    binding.CloseTimeout = new TimeSpan(1, 0, 0, 0);
                    binding.OpenTimeout = new TimeSpan(1, 0, 0, 0);
                    binding.ReceiveTimeout = new TimeSpan(1, 0, 0, 0);
                    binding.SendTimeout = new TimeSpan(1, 0, 0, 0);
                    binding.MaxReceivedMessageSize = Int32.MaxValue;

                    //ChannelFactory<IOMSClient> factoryToTMS = new ChannelFactory<IOMSClient>(binding,
                    //    new EndpointAddress("net.tcp://localhost:6080/TransactionManagerService"));
                    ////ProxyToOMS = factoryToTMS.CreateChannel();
                    //TMSAnswerToClient answerFromTransactionManager = new TMSAnswerToClient();

                    //try
                    //{
                    //    //answerFromTransactionManager = ProxyToOMS.GetNetwork("");
                    //}
                    //catch (Exception e) { }

                    //InitNetwork();
                    //InitElementsAndProperties(answerFromTransactionManager);
                    //DrawElementsOnGraph(answerFromTransactionManager.GraphDeep);

                    return;
                }

                int i = 0;
                foreach (UIUpdateModel sum in update)
                {
                    ElementProperties property;
                    properties.TryGetValue(sum.Gid, out property);
                    if (property != null)
                    {
                        property.IsEnergized = sum.IsEnergized;

                        if (property is BreakerProperties && i == 0)
                        {
                            BreakerProperties breakerProperties = property as BreakerProperties;
                            breakerProperties.State = sum.State;

                            if (sum.State == OMSSCADACommon.States.CLOSED)
                            {
                                breakerProperties.CanCommand = false;
                            }

                            Measurement measurement;
                            DigitalMeasurement digitalMeasurement;
                            try
                            {
                                Measurements.TryGetValue(property.Measurements[0].GID, out measurement);
                                digitalMeasurement = (DigitalMeasurement)measurement;
                            }
                            catch (Exception)
                            {
                                continue;
                            }

                            if (digitalMeasurement != null)
                            {
                                digitalMeasurement.State = sum.State;
                            }
                        }
                        if (property is EnergyConsumerProperties)
                        {
                            EnergyConsumerProperties energyConsumerProperties = property as EnergyConsumerProperties;
                            energyConsumerProperties.Call = false;
                        }
                    }
                    i++;
                }
            }
        }

        private void GetAnalogUpdate(List<UIUpdateModel> update)
        {
            if (update != null)
            {
                AnalogMeasurement analogMeasurement = (AnalogMeasurement)this.Measurements.Where(meas => meas.Value.GID == update[0].Gid).FirstOrDefault().Value;

                if (analogMeasurement != null)
                {
                    analogMeasurement.Value = update[0].AnValue;
                }
            }
        }

        private void GetCrewUpdate(UIUpdateModel update)
        {
            Console.WriteLine(update.Response.ToString());
        }

        private void GetIncident(IncidentReport report)
        {
            IncidentReport temp = new IncidentReport();
            bool found = false;
            foreach (IncidentReport ir in IncidentReports)
            {
                if (DateTime.Compare(ir.Time, report.Time) == 0)
                {
                    temp = ir;
                    found = true;
                    break;
                }
            }
            if (found)
            {
                temp.Reason = report.Reason;
                temp.RepairTime = report.RepairTime;
                temp.IncidentState = report.IncidentState;
                temp.Crewtype = report.Crewtype;
                temp.MaxValue = report.MaxValue;
                temp.CurrentValue = report.CurrentValue;

                if (report.InvestigationCrew != null && report.InvestigationCrew.CrewName != null)
                {
                    temp.InvestigationCrew = this.Crews.Where(c => c.CrewName == report.InvestigationCrew.CrewName).FirstOrDefault();
                    temp.InvestigationCrew.Working = report.InvestigationCrew.Working;
                }
                if (report.RepairCrew != null && report.RepairCrew.CrewName != null)
                {
                    temp.RepairCrew = this.Crews.Where(c => c.CrewName == report.RepairCrew.CrewName).FirstOrDefault();
                    temp.RepairCrew.Working = report.RepairCrew.Working;
                }

                RaisePropertyChanged("Crews");

                //if (temp.IncidentState == IncidentState.READY_FOR_REPAIR)
                //{
                //    if (temp.InvestigationCrew != null)
                //    {
                //        temp.InvestigationCrew.Working = false;
                //        RaisePropertyChanged("Crews");
                //    }
                //}
                //else if (temp.IncidentState == IncidentState.REPAIRED)
                //{
                //    if (temp.RepairCrew != null)
                //    {
                //        temp.RepairCrew.Working = false;
                //        RaisePropertyChanged("Crews");
                //    }
                //}
            }
            else
            {
                IncidentReports.Insert(0, report);
            }

            try
            {
                ElementProperties element = Properties.Where(p => p.Value.MRID == report.MrID).FirstOrDefault().Value;
                if (report.IncidentState == IncidentState.REPAIRED)
                {
                    element.Incident = false;
                    if (element.IsUnderScada)
                    {
                        element.CanCommand = true;
                    }
                }
                else
                {
                    element.Incident = true;
                }

                if (report.IncidentState == IncidentState.INVESTIGATING || report.IncidentState == IncidentState.REPAIRING)
                {
                    element.CrewSent = true;

                    tokenSource = new CancellationTokenSource();
                    CancellationToken token = tokenSource.Token;
                    Task.Factory.StartNew(() => ProgressBarChange(temp, token), token);
                }
                else
                {
                    element.CrewSent = false;
                }
            }
            catch { }
        }

        private void GetCallFromConsumers(UIUpdateModel call)
        {
            ElementProperties property;
            properties.TryGetValue(call.Gid, out property);

            EnergyConsumerProperties consumerProperties = property as EnergyConsumerProperties;

            if (consumerProperties != null)
            {
                consumerProperties.IsEnergized = call.IsEnergized;
                consumerProperties.Call = true;
            }
        }

        private void SearchForIncident(bool isIncident, long incidentBreaker)
        {
            ElementProperties propBr;
            properties.TryGetValue(incidentBreaker, out propBr);
            if (propBr != null)
            {
                try
                {
                    if (isIncident == false)
                    {
                        tokenSource.Cancel();
                        Thread.Sleep(50);

                        tokenSource = new CancellationTokenSource();
                        CancellationToken token = tokenSource.Token;

                        blinkTask = Task.Factory.StartNew(() => Blink(propBr, token), token);
                        propBr.IsCandidate = true;
                    }
                    else if (isIncident)
                    {
                        tokenSource.Cancel();
                        Thread.Sleep(50);

                        tokenSource = new CancellationTokenSource();
                        CancellationToken token = tokenSource.Token;

                        blinkTask = Task.Factory.StartNew(() => FinalCandidate(propBr, token), token);
                        propBr.IsCandidate = true;
                    }
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        private async Task ProgressBarChange(IncidentReport report, CancellationToken ct)
        {
            while (true)
            {
                await Task.Delay(1000);

                if (report.CurrentValue != report.MaxValue)
                {
                    report.CurrentValue++;
                }
                else
                {
                    return;
                }

                if (ct.IsCancellationRequested)
                {
                    ct.ThrowIfCancellationRequested();
                }
            }
        }

        private async Task Blink(ElementProperties sw, CancellationToken ct)
        {
            while (true)
            {
                await Task.Delay(800);
                sw.IsCandidate = sw.IsCandidate == true ? false : true;

                if (ct.IsCancellationRequested)
                {
                    sw.IsCandidate = false;
                    ct.ThrowIfCancellationRequested();
                }
            }
        }
        private async Task FinalCandidate(ElementProperties sw, CancellationToken ct)
        {
            int i = 0;
            while (true)
            {
                if (i == 20)
                {
                    tokenSource.Cancel();
                }
                await Task.Delay(300);
                sw.IsCandidate = sw.IsCandidate == true ? false : true;

                if (ct.IsCancellationRequested)
                {
                    sw.IsCandidate = false;
                    ct.ThrowIfCancellationRequested();
                }
                i++;
            }
        }
        #endregion

        #region Execute open commands
        private void ExecuteOpenNetworkExplorerCommand()
        {
            NetworkExplorerViewModel nevm = new NetworkExplorerViewModel();
            if (nevm.IsOpen == false)
            {
                nevm.IsOpen = true;

                NetworkExplorer networkExplorer = new NetworkExplorer();
                
                networkExplorer.DataContext = nevm;

                nevm.GetAllSources(this);

                ShellFillerShell sfs = new ShellFillerShell() { DataContext = this };

                sfs.MainScroll.Content = networkExplorer;
                sfs.Header.Text = "Network Explorer";

                PlaceOrFocusControlInShell(NetworkExplorerViewModel.Position, sfs, false, null);

                return;
            }

            PlaceOrFocusControlInShell(NetworkExplorerViewModel.Position, null, true, "Network Explorer");
        }

        private void ExecuteOpenIncidentExplorerCommand()
        {
            IncidentExplorerViewModel ievm = new IncidentExplorerViewModel();
            if (ievm.IsOpen == false)
            {
                ievm.IsOpen = true;

                IncidentExplorer incidentExplorer = new IncidentExplorer
                {
                    DataContext = ievm
                };

                ievm.GetAllIncidentReports();

                ShellFillerShell sfs = new ShellFillerShell() { DataContext = this };

                sfs.MainScroll.Content = incidentExplorer;
                sfs.Header.Text = "Incident Explorer";

                PlaceOrFocusControlInShell(IncidentExplorerViewModel.Position, sfs, false, null);

                return;
            }

            PlaceOrFocusControlInShell(IncidentExplorerViewModel.Position, null, true, "Incident Explorer");
        }

        private void ExecuteOpenPropertiesCommand()
        {

        }

        private void ExecuteOpenOutputCommand()
        {

        }

        private void ExecuteOpenReportExplorerCommand()
        {
            ReportExplorerModelView revm = new ReportExplorerModelView();
            if (revm.IsOpen == false)
            {
                revm.IsOpen = true;

                ReportExplorer reportExplorer = new ReportExplorer();
                
                reportExplorer.DataContext = revm;

                ShellFillerShell sfs = new ShellFillerShell() { DataContext = this };

                sfs.MainScroll.Content = reportExplorer;
                sfs.Header.Text = "Report Explorer";

                PlaceOrFocusControlInShell(ReportExplorerModelView.Position, sfs, false, null);

                return;
            }

            PlaceOrFocusControlInShell(ReportExplorerModelView.Position, null, true, "Report Explorer");
        }

        private void ExecuteOpenNetworkViewCommand(object parameter)
        {
            bool isOpen = false;

            if (!isSourceOpen.TryGetValue((string)parameter, out isOpen))
            {
                isSourceOpen.Add((string)parameter, true);
            }

            if (!isOpen)
            {
                isSourceOpen[(string)parameter] = true;

                NetworkViewControl networkViewExplorer = new NetworkViewControl();
                NetworkViewViewModel nvevm = new NetworkViewViewModel();
                networkViewExplorer.DataContext = nvevm;

                nvevm.GetNetwork((string)parameter);

                ShellFillerShell sfs = new ShellFillerShell();

                sfs.MainScroll.Content = networkViewExplorer;
                sfs.Header.Text = (string)parameter;

                PlaceOrFocusControlInShell(NetworkViewViewModel.Position, sfs, false, null);

                return;
            }

            PlaceOrFocusControlInShell(NetworkViewViewModel.Position, null, true, (string)parameter);
        }

        private void PlaceOrFocusControlInShell(ShellPosition position, ShellFillerShell sfs, bool isFocus, string parameter)
        {
            var currentTabControl = ShellProperties[position];

            if (!isFocus)
            {
                TabItem tabItem = new TabItem() { Header = sfs.Header.Text };

                tabItem.Content = sfs;
                tabItem.Header = sfs.Header.Text;

                currentTabControl.TabControlTabs.Add(tabItem);
                currentTabControl.TabControlIndex = currentTabControl.TabControlTabs.Count - 1;
                RaisePropertyChanged("ShellProperties");

                if (position != ShellPosition.CENTER)
                {
                    currentTabControl.TabControlVisibility = Visibility.Visible;
                }
                else
                {
                    sfs.Header.Text = "";
                }
            }
            else
            {
                int i = 0;
                for (i = 0; i < currentTabControl.TabControlTabs.Count; i++)
                {
                    if ((string)currentTabControl.TabControlTabs[i].Header == parameter)
                    {
                        break;
                    }
                }

                currentTabControl.TabControlIndex = i;
            }
        }
        #endregion

        #region Execute close commands

        private void ExecuteCloseControlCommand(object parameter)
        {
            var header = ((object[])parameter)[0];
            var position = ((object[])parameter)[1];

            var properties = this.ShellProperties[(ShellPosition)position];

            properties.TabControlTabs.Remove(properties.TabControlTabs.Where(tab => tab.Header == header).First());

            if (properties.TabControlTabs.Count == 0)
            {
                properties.TabControlVisibility = Visibility.Collapsed;
            }

            try
            {
                SingleShellFillerViewModel viewModel = (SingleShellFillerViewModel)((object[])parameter)[2];
                viewModel.IsOpen = false;
            }
            catch { }
            finally
            {
                ((object[])parameter)[2] = null;
            }

            bool isOpen = false;
            if (isSourceOpen.TryGetValue((string)header, out isOpen))
            {
                isSourceOpen[(string)header] = false;
            }
        }
        #endregion
    }
}


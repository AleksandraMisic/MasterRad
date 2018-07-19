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
        private ClientDMSProxy dMSProxy;
        private ClientIMSProxy iMSProxy;

        private static Dictionary<string, bool> isSourceOpen = null;

        public Task blinkTask;

        public CancellationTokenSource tokenSource = new CancellationTokenSource();

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

        private int commandsIndex = 0;
        private bool test = true;

        private ObservableCollection<IncidentReport> incidentReports = new ObservableCollection<IncidentReport>();
        private ObservableCollection<Crew> crews = new ObservableCollection<Crew>();

        private double startHeight = 20;
        private double startWidth = 3;
        private double currentHeight = 20;
        private double currentWidth = 3;
        #endregion

        #region Commands
        private RelayCommand openNetworkExplorerCommand;
        private RelayCommand openIncidentExplorerCommand;
        private RelayCommand openReportExplorerCommand;
        private RelayCommand openOutputCommand;

        private RelayCommand closeControlCommand;

        private RelayCommand sendCrewCommand;

        private RelayCommand executeSwitchCommand;

        private RelayCommand generateIncidentByDateChartCommand;
        private RelayCommand generateIncidentByBreakerChartCommand;
        private RelayCommand generateStatesByBreakerChartCommand;
        #endregion

        #region Constructor
        static MainShellViewModel()
        {
            isSourceOpen = new Dictionary<string, bool>();
        }

        public MainShellViewModel()
        {
            TopMenu = new ObservableCollection<UserControl>();
            TopMenu.Add(new TopMenu());

            //subscriber = new Subscriber();
            //subscriber.Subscribe();
            //subscriber.publishCrewEvent += GetCrewUpdate;
            //subscriber.publishIncident += GetIncident;
            //subscriber.publishCall += GetCallFromConsumers;
            //subscriber.publiesBreakers += SearchForIncident;

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
        
        #endregion

        #region Command execution
        public RelayCommand GenerateIncidentByDateChartCommand
        {
            get
            {
                return generateIncidentByDateChartCommand ?? new RelayCommand(
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
                return generateIncidentByBreakerChartCommand ?? new RelayCommand(
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
                return generateStatesByBreakerChartCommand ?? new RelayCommand(
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

        public RelayCommand CloseControlCommand
        {
            get
            {
                return closeControlCommand ?? new RelayCommand(
                    (parameter) =>
                    {
                        ExecuteCloseControlCommand(parameter);
                    });
            }
        }

        public RelayCommand SendCrewCommand
        {
            get
            {
                return sendCrewCommand ?? new RelayCommand(
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
                return executeSwitchCommand ?? new RelayCommand(
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

                //foreach (Switch breaker in this.Breakers)
                //{
                //    mrids.Add(breaker.MRID);
                //}

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
            //Measurement measurement = this.Measurements.Where(m => m.Value.MRID == (string)parameter).FirstOrDefault().Value;
            //if (measurement != null)
            //{
            //    ElementProperties elementProperties;
            //    Properties.TryGetValue(measurement.Psr, out elementProperties);

            //    if (elementProperties != null)
            //    {
            //        elementProperties.CanCommand = false;
            //    }
            //}

            //try
            //{
            //    //ProxyToOMS.SendCommandToSCADA(TypeOfSCADACommand.WriteDigital, (string)parameter, CommandTypes.CLOSE, 0);
            //}
            //catch { }
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

        public static Dictionary<string, bool> IsSourceOpen
        {
            get
            {
                return isSourceOpen;
            }
            set
            {
                isSourceOpen = value;
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

            //try
            //{
            //    ElementProperties element = Properties.Where(p => p.Value.MRID == report.MrID).FirstOrDefault().Value;
            //    if (report.IncidentState == IncidentState.REPAIRED)
            //    {
            //        element.Incident = false;
            //        if (element.IsUnderScada)
            //        {
            //            element.CanCommand = true;
            //        }
            //    }
            //    else
            //    {
            //        element.Incident = true;
            //    }

            //    if (report.IncidentState == IncidentState.INVESTIGATING || report.IncidentState == IncidentState.REPAIRING)
            //    {
            //        element.CrewSent = true;

            //        tokenSource = new CancellationTokenSource();
            //        CancellationToken token = tokenSource.Token;
            //        Task.Factory.StartNew(() => ProgressBarChange(temp, token), token);
            //    }
            //    else
            //    {
            //        element.CrewSent = false;
            //    }
            //}
            //catch { }
        }

        private void GetCallFromConsumers(UIUpdateModel call)
        {
            //ElementProperties property;
            //properties.TryGetValue(call.Gid, out property);

            //EnergyConsumerProperties consumerProperties = property as EnergyConsumerProperties;

            //if (consumerProperties != null)
            //{
            //    consumerProperties.IsEnergized = call.IsEnergized;
            //    consumerProperties.Call = true;
            //}
        }

        private void SearchForIncident(bool isIncident, long incidentBreaker)
        {
            //ElementProperties propBr;
            //properties.TryGetValue(incidentBreaker, out propBr);
            //if (propBr != null)
            //{
            //    try
            //    {
            //        if (isIncident == false)
            //        {
            //            tokenSource.Cancel();
            //            Thread.Sleep(50);

            //            tokenSource = new CancellationTokenSource();
            //            CancellationToken token = tokenSource.Token;

            //            blinkTask = Task.Factory.StartNew(() => Blink(propBr, token), token);
            //            propBr.IsCandidate = true;
            //        }
            //        else if (isIncident)
            //        {
            //            tokenSource.Cancel();
            //            Thread.Sleep(50);

            //            tokenSource = new CancellationTokenSource();
            //            CancellationToken token = tokenSource.Token;

            //            blinkTask = Task.Factory.StartNew(() => FinalCandidate(propBr, token), token);
            //            propBr.IsCandidate = true;
            //        }
            //    }
            //    catch (Exception)
            //    {
            //        return;
            //    }
            //}
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

                nevm.GetAllSources();

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
        #endregion

        #region Execute close commands

        private void ExecuteCloseControlCommand(object parameter)
        {
            var header = ((object[])parameter)[0];
            var position = ((object[])parameter)[1];

            var properties = ShellProperties[(ShellPosition)position];

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


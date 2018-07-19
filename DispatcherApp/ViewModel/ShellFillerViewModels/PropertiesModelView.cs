using DispatcherApp.Model.Measurements;
using DMSCommon;
using OMSSCADACommon;
using PubSubscribe;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using UIShell.Model;
using UIShell.ViewModel;

namespace DispatcherApp.ViewModel.ShellFillerModelViews
{
    public class PropertiesModelView : SingleShellFillerViewModel
    {
        private Subscriber subscriber;

        private static bool isOpen;
        private static ShellPosition position;
        private string elementMRID;
        private Visibility measurementVisibility;
        private Visibility digitalMeasurementVisibility;
        private Visibility analogMeasurementVisibility;

        private ObservableCollection<UserControl> digitalControls = null;
        private ObservableCollection<UserControl> analogControls = null;

        private static Dictionary<string, Measurement> measurements;

        private RelayCommand openPropertiesCommand;

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

        public string ElementMRID
        {
            get { return elementMRID; }
            set { elementMRID = value; }
        }

        public Visibility MeasurementVisibility
        {
            get { return measurementVisibility; }
            set { measurementVisibility = value; }
        }

        public Visibility DigitalMeasurementVisibility
        {
            get { return digitalMeasurementVisibility; }
            set { digitalMeasurementVisibility = value; }
        }

        public Visibility AnalogMeasurementVisibility
        {
            get { return analogMeasurementVisibility; }
            set { analogMeasurementVisibility = value; }
        }

        public ObservableCollection<UserControl> DigitalControls
        {
            get { return digitalControls; }
            set { digitalControls = value; }
        }

        public ObservableCollection<UserControl> AnalogControls
        {
            get { return analogControls; }
            set { analogControls = value; }
        }

        public static Dictionary<string, Measurement> Measurements
        {
            get { return measurements; }
            set { measurements = value; }
        }

        static PropertiesModelView()
        {
            Position = ShellPosition.RIGHT;
        }

        public PropertiesModelView()
        {
            digitalControls = new ObservableCollection<UserControl>();
            analogControls = new ObservableCollection<UserControl>();

            measurements = new Dictionary<string, Measurement>();

            subscriber = new Subscriber();
            subscriber.Subscribe();
            subscriber.publishDigitalUpdateEvent += GetDigitalUpdate;
            subscriber.publishAnalogUpdateEvent += GetAnalogUpdate;
        }

        private static void GetDigitalUpdate(string mrid, States state)
        {
            measurements.TryGetValue(mrid, out Measurement measurement);

            DigitalMeasurement digitalMeasurement = measurement as DigitalMeasurement;
            digitalMeasurement.State = state;
        }

        private static void GetAnalogUpdate(string mrid, float value)
        {
            measurements.TryGetValue(mrid, out Measurement measurement);

            AnalogMeasurement analogMeasurement = measurement as AnalogMeasurement;
            analogMeasurement.Value = value;
        }
    }
}

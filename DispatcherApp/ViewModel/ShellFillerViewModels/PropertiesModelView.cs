using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using UIShell.Model;
using UIShell.ViewModel;

namespace DispatcherApp.ViewModel.ShellFillerModelViews
{
    public class PropertiesModelView : SingleShellFillerViewModel
    {
        private static bool isOpen;
        private static ShellPosition position;
        private Visibility measurementVisibility;
        private Visibility digitalMeasurementVisibility;
        private Visibility analogMeasurementVisibility;

        ObservableCollection<UserControl> digitalControls = null;
        ObservableCollection<UserControl> analogControls = null;

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

        static PropertiesModelView()
        {
            Position = ShellPosition.RIGHT;
        }

        public PropertiesModelView()
        {
            digitalControls = new ObservableCollection<UserControl>();
            analogControls = new ObservableCollection<UserControl>();
        }
    }
}

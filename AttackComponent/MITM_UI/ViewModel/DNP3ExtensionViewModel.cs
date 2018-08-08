using DNP3DataPointsModel;
using MITM_UI.Model.GlobalInfo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UIShell.Model;
using UIShell.ViewModel;

namespace MITM_UI.ViewModel
{
    class DNP3ExtensionViewModel : SingleShellFillerViewModel
    {
        private static bool isOpen;
        private static ShellPosition position;
        private bool isConfigPresent;

        public ObservableCollection<AnalogInputPoint> AnalogInputPoints { get; set; }

        public static ShellPosition Position
        {
            get { return position; }
            set { position = value; }
        }

        public bool IsConfigPresent
        {
            get
            {
                return isConfigPresent;
            }
            set
            {
                isConfigPresent = value;
                RaisePropertyChanged("IsConfigPresent");
            }
        }

        public DNP3ExtensionViewModel()
        {
            Position = ShellPosition.CENTER;
            AnalogInputPoints = new ObservableCollection<AnalogInputPoint>();

            foreach (AnalogInputPoint analog in Database.AnalogInputPoints.Values)
            {
                AnalogInputPoints.Add(analog);
            }
        }

        public void AnalogInputChanged(int index, int value)
        {

        }

        public override bool IsOpen
        {
            get { return isOpen; }
            set { isOpen = value; }
        }
    }
}

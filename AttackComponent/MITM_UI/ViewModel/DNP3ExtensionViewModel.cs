using DNP3DataPointsModel;
using DNP3TCPDriver.UserLevel;
using MITM_Common;
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
    public class DNP3ExtensionViewModel : SingleShellFillerViewModel
    {
        private static bool isOpen;
        private static ShellPosition position;
        private bool isConfigPresent;

        MITMServiceProxy mITMServiceProxy = null;

        public ObservableCollection<AnalogInputPoint> AnalogInputPoints { get; set; }

        private RelayCommand modifyCommand;
        private RelayCommand acquireConfigurationCommand;

        public static ShellPosition Position
        {
            get { return position; }
            set { position = value; }
        }

        public RelayCommand ModifyCommand
        {
            get
            {
                return modifyCommand ?? new RelayCommand(
                    (parameter) =>
                    {
                        ExecuteModifyCommand(parameter);
                    });
            }
        }

        public RelayCommand AcquireConfigurationCommand
        {
            get
            {
                return acquireConfigurationCommand ?? new RelayCommand(
                    (parameter) =>
                    {
                        ExecuteAcquireConfigurationCommand(parameter);
                    });
            }
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
            mITMServiceProxy = new MITMServiceProxy(NetTcpBindingCreator.Create());

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

        private void ExecuteModifyCommand(object parameter)
        {
            AnalogInputPoint analogInputPoint = (AnalogInputPoint)(((object[])parameter)[1]);

            if ((string)(((object[])parameter)[0]) == "Fix Value")
            {
                AnalogInputPoints.Where(a => a.Index == analogInputPoint.Index).FirstOrDefault().IsFixed = true;

                mITMServiceProxy.FixValue(PointType.ANALOG_INPUT, analogInputPoint.Index);
            }
            else
            {
                AnalogInputPoints.Where(a => a.Index == analogInputPoint.Index).FirstOrDefault().IsFixed = false;

                mITMServiceProxy.ReleaseValue(PointType.ANALOG_INPUT, analogInputPoint.Index);
            }
        }

        private void ExecuteAcquireConfigurationCommand(object parameter)
        {
            mITMServiceProxy.AcquireOutstationConfiguration();
        }
    }
}

using DNP3DataPointsModel;
using DNP3TCPDriver.UserLevel;
using MITM_Common;
using MITM_Common.MITM_Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIShell.ViewModel;

namespace MITM_UI.Model.GlobalInfo
{
    public class Database
    {
        public static GlobalConnectionInfo GlobalConnectionInfo { get; set; }

        public static Dictionary<ViewModelType, SingleShellFillerViewModel> ViewModels;

        public static Dictionary<int, AnalogInputPoint> AnalogInputPoints { get; set; }

        public static object lockObject;

        public static Dictionary<Tuple<int, PointType>, FixedValue> FixedValues { get; set; }

        static Database()
        {
            GlobalConnectionInfo = new GlobalConnectionInfo();
            ViewModels = new Dictionary<ViewModelType, SingleShellFillerViewModel>();
            AnalogInputPoints = new Dictionary<int, AnalogInputPoint>();
            FixedValues = new Dictionary<Tuple<int, PointType>, FixedValue>();
            lockObject = new object();
        }
    }
}

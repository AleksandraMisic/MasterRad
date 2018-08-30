using DNP3DataPointsModel;
using DNP3TCPDriver.UserLevel;
using MITM_Common.MITM_Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MITM_Common;

namespace MITM_Service
{
    public class Database
    {
        public static GlobalConnectionInfo GlobalConnectionInfo { get; set; }

        public static ARPSpoofParticipantsInfo ARPSpoofParticipantsInfo { get; set; }

        public static Dictionary<int, AnalogInputPoint> AnalogInputPoints { get; set; }

        public static Dictionary<Tuple<int, PointType>, FixedValue> FixedValues { get; set; }

        public static bool IsAttack { get; set; }

        public static object lockObject;

        public static bool IsConfigAck { get; set; }

        static Database()
        {
            GlobalConnectionInfo = new GlobalConnectionInfo();
            AnalogInputPoints = new Dictionary<int, AnalogInputPoint>();
            ARPSpoofParticipantsInfo = new ARPSpoofParticipantsInfo();
            FixedValues = new Dictionary<Tuple<int, PointType>, FixedValue>();
            lockObject = new object();
        }
    }
}

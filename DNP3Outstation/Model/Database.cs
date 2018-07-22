using DNP3DataPointsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3Outstation.Model
{
    public class Database
    {
        public static List<AnalogInputPoint> AnalogInputPoints { get; set; }
        public static object DatabaseLock { get; set; }

        static Database()
        {
            AnalogInputPoints = new List<AnalogInputPoint>();
            DatabaseLock = new object();
        }
    }
}

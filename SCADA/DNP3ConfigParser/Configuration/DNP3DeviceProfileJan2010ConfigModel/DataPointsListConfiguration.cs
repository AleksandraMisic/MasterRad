using DNP3DataPointsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3ConfigParser.Configuration.DNP3DeviceProfileJan2010ConfigModel
{
    public class DataPointsListConfiguration
    {
        public List<AnalogInputPoint> AnalogInputPoints { get; set; }

        public DataPointsListConfiguration()
        {
            AnalogInputPoints = new List<AnalogInputPoint>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3ConfigParser.Configuration.DNP3DeviceProfileJan2010ConfigModel.Points
{
    public class AnalogInputPoint
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public int ChangeEventClass { get; set; }
        public int MinIntegerTransmittedValue { get; set; }
        public int MaxIntegerTransmittedValue { get; set; }
        public int ScaleFactor { get; set; }
        public double ScaleOffset { get; set; }
        public string Units { get; set; }
        public int Resolution { get; set; }
        public string Description { get; set; }
    }
}

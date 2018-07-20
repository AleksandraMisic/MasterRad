using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.RealtimeDatabase.Model
{
    public class DNP3Device
    {
        public string DeviceName { get; set; }

        public List<ProcessVariable> ProcessVariables { get; set; }

        public DNP3Device()
        {
            ProcessVariables = new List<ProcessVariable>(); 
        }
    }
}

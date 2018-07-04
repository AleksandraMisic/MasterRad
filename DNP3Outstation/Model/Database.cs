using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3Outstation.Model
{
    public class Database
    {
        static Database()
        {

        }

        public static List<Device> Devices
        {
            get;
            set;
        }
    }
}

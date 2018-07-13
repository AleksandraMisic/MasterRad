using MITM_Common.MITM_Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITM_Common
{
    public class Database
    {
        public static GlobalConnectionInfo GlobalConnectionInfo { get; set; }

        static Database()
        {
            GlobalConnectionInfo = new GlobalConnectionInfo();
        }
    }
}

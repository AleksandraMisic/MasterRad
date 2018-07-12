using MITM_Common.MITM_Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MITM_Service
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "MITM Service";
            MITM_ServiceManager msm = new MITM_ServiceManager();

            GetConnectionInfo getConnectionInfo = new GetConnectionInfo();

            TaskFactory taskFactory = new TaskFactory();
            taskFactory.StartNew(() => getConnectionInfo.Get());

            msm.Start();

            Console.ReadLine();

            msm.Stop();
        }
    }
}

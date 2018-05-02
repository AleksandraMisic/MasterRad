using DMSCommon.Model;
using DMSService;
using DMSServiceHost.Hosts;
using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMSServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title="Distribution Management System";

            try
            {
                DMSService.DMSService.Instance.Start();

                ClientHost clientHost = new ClientHost();

                clientHost.Start();

                Console.ReadLine();

                clientHost.Stop();
            }
            catch { }
        }
    }
}

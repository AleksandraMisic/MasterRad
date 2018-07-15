using DMSCommon.Model;
using DMSService;
using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMS.Hosts;

namespace DMS
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title="Distribution Management System";

            try
            {
                DMSServiceHost.Instance.Start();

                Console.ReadLine();

                //DMSServiceHost.Instance.Stop();
            }
            catch(Exception e) { }
        }
    }
}

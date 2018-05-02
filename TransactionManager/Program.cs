using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TransactionManager.Hosts;
using TransactionManager.ServiceImplementations;
using TransactionManagerContract;

namespace TransactionManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Transaction Manager";

            try
            {
                DistributedTransactionHost distributedTransactionHost = new DistributedTransactionHost();

                ClientGeneralHost clientGeneralHost = new ClientGeneralHost();
                ClientDMSHost clientDMSHost = new ClientDMSHost();

                distributedTransactionHost.Start();

                clientGeneralHost.Start();
                clientDMSHost.Start();

                Console.ReadLine();

                distributedTransactionHost.Stop();

                clientGeneralHost.Stop();
                clientDMSHost.Stop();

            }
            catch(Exception e) { }
        }
    }
}

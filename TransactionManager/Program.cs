using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TransactionManager.Hosts;
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
                ClientDMSHost clientDMSHost = new ClientDMSHost();
                ClientNMSHost clientNMSHost = new ClientNMSHost();

                distributedTransactionHost.Start();
                clientDMSHost.Start();
                clientNMSHost.Start();

                Console.ReadLine();

                distributedTransactionHost.Stop();
                
                clientDMSHost.Stop();
                clientNMSHost.Stop();

            }
            catch(Exception e) { }
        }
    }
}

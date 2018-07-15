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

                distributedTransactionHost.Start();
                
                clientDMSHost.Start();

                Console.ReadLine();

                distributedTransactionHost.Stop();
                
                clientDMSHost.Stop();

            }
            catch(Exception e) { }
        }
    }
}

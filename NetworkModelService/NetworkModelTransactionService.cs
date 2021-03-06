﻿using FTN.Common;
using System;
using System.ServiceModel;
using TransactionManagerContract;

namespace FTN.Services.NetworkModelService
{

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class NetworkModelTransactionService : IDistributedTransaction
    {
        private static GenericDataAccess gda = new GenericDataAccess();

        public void Enlist()
        {
            IDistributedTransactionCallback callback = OperationContext.Current.GetCallbackChannel<IDistributedTransactionCallback>();
            Console.WriteLine("Pozvan je enlist na NMS-u");
            try
            {
                gda.GetCopyOfNetworkModel();
                callback.CallbackEnlist(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                callback.CallbackEnlist(false);
            }
        }

        public void Prepare(Delta delta)
        {
            Console.WriteLine("Pozvan je prepare na NMS-u");
            IDistributedTransactionCallback callback = OperationContext.Current.GetCallbackChannel<IDistributedTransactionCallback>();

            try
            {
                UpdateResult updateResult = gda.ApplyUpdate(delta);
                if (updateResult.Result == ResultType.Succeeded)
                {
                    callback.CallbackPrepare(true);
                }
                else
                {
                    //Rollback();
                    callback.CallbackPrepare(false);
                }
            }
            catch (Exception ex)
            {
                //Rollback();
                callback.CallbackPrepare(false); // problematicno ako se desi exception u pozivu, nece se uhvatiti
                Console.WriteLine(ex.Message);
            }
        }

        public void Commit()
        {
            Console.WriteLine("Pozvan je Commit na NMS-u");

            if (GenericDataAccess.NewNetworkModel != null)
            {
                GenericDataAccess.NetworkModel = GenericDataAccess.NewNetworkModel;
                ResourceIterator.NetworkModel = GenericDataAccess.NewNetworkModel;
            }

            IDistributedTransactionCallback callback = OperationContext.Current.GetCallbackChannel<IDistributedTransactionCallback>();
            callback.CallbackCommit("Uspjesno je prosao commit na NMS-u");
        }
            
        public void Rollback()
        {
            Console.WriteLine("Pozvan je RollBack na NMSu");
            GenericDataAccess.NewNetworkModel = null;
            GenericDataAccess.NetworkModel = GenericDataAccess.OldNetworkModel;
            ResourceIterator.NetworkModel = GenericDataAccess.OldNetworkModel;
            IDistributedTransactionCallback callback = OperationContext.Current.GetCallbackChannel<IDistributedTransactionCallback>();
            callback.CallbackRollback("Something went wrong on NMS");
        }
    }
}

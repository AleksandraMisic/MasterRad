using FTN.Common;
using PubSubscribe;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using TransactionManagerContract;
using DMSCommon.Model;
using DMSCommon.TreeGraph;
using DMSCommon;
using DMS.Hosts;

namespace DMSService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class DMSTransactionService : IDistributedTransaction
    {
        private static Tree<Element> newTree;
        private static Tree<Element> oldTree;

        public void Enlist()
        {
            Console.WriteLine("Pozvan je enlist na DMS-u");
            oldTree = DMS.Hosts.DMSServiceHost.Instance.Tree;
            IDistributedTransactionCallback callback = OperationContext.Current.GetCallbackChannel<IDistributedTransactionCallback>();
            callback.CallbackEnlist(true);
        }

        public void Commit()
        {
            Console.WriteLine("Pozvan je Commit na DMS-u");
            DMSServiceHost.Instance.Tree = newTree;
            if (DMSServiceHost.updatesCount >= 2)
            {
                Publisher publisher = new Publisher();
                List<UIUpdateModel> update = new List<UIUpdateModel>();
                Source s = (Source)DMSServiceHost.Instance.Tree.Data[DMSServiceHost.Instance.Tree.Roots[0]];
                update.Add(new UIUpdateModel(true, s.ElementGID));

                publisher.PublishUpdateDigital(update);
            }


            IDistributedTransactionCallback callback = OperationContext.Current.GetCallbackChannel<IDistributedTransactionCallback>();
            callback.CallbackCommit("Uspjesno je prosao commit na DMS-u");
        }

        public void Prepare(Delta delta)
        {
            Console.WriteLine("Pozvan je prepare na DMS-u");

            newTree = DMSServiceHost.Instance.InitializeNetwork(delta);
            DMSServiceHost.updatesCount += 1;
            IDistributedTransactionCallback callback = OperationContext.Current.GetCallbackChannel<IDistributedTransactionCallback>();

            if (newTree.Data.Values.Count != 0)
            {
                callback.CallbackPrepare(true);
            }
            else
            {
                callback.CallbackPrepare(false);
            }
        }

        public void Rollback()
        {
            Console.WriteLine("Pozvan je RollBack na DMSu");
            newTree = null;
            DMSServiceHost.Instance.Tree = oldTree;
            IDistributedTransactionCallback callback = OperationContext.Current.GetCallbackChannel<IDistributedTransactionCallback>();
            callback.CallbackRollback("Something went wrong on DMS");
        }
    }
}
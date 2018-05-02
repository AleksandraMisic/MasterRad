using FTN.Common;
using System.ServiceModel;

namespace TransactionManagerContract
{
    [ServiceContract(CallbackContract = typeof(IDistributedTransactionCallback))]
    public interface IDistributedTransaction
    {
        [OperationContract]
        void Enlist();

        [OperationContract]
        void Prepare(Delta delta);

        [OperationContract]
        void Commit();

        [OperationContract]
        void Rollback();
    }
}

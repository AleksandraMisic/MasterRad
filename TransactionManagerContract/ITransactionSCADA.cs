using OMSSCADACommon;
using System.ServiceModel;

namespace TransactionManagerContract
{
    [ServiceContract(CallbackContract = typeof(IDistributedTransactionCallback))]
    public interface ITransactionSCADA
    {
        [OperationContract]
        void Enlist();

        [OperationContract]
        void Prepare(ScadaDelta delta);

        [OperationContract]
        void Commit();

        [OperationContract]
        void Rollback();
    }
}

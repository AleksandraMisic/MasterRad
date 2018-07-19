using OMSSCADACommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TransactionManagerContract.ClientSCADA
{
    [ServiceContract]
    public interface IClientSCADA
    {
        [OperationContract]
        void GetDiscreteMeasurement(string mrid, out States state);

        [OperationContract]
        void GetAnalogMeasurement(string mrid, out float value);
    }
}

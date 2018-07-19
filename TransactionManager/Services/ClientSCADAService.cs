using OMSSCADACommon;
using OMSSCADACommon.Commands;
using OMSSCADACommon.Responses;
using SCADA.ClientHandler;
using SCADAContracts;
using TransactionManagerContract.ClientSCADA;

namespace TransactionManager.Services
{
    public class ClientSCADAService : IClientSCADA
    {
        private static SCADAProxy sCADAProxy;

        static ClientSCADAService()
        {
            sCADAProxy = new SCADAProxy();
        }

        public void GetAnalogMeasurement(string mrid, out float value)
        {
            Command command = new ReadSingleAnalog() { Id = mrid };
            Response response = sCADAProxy.ExecuteCommand(command);

            value = ((AnalogVariable)response.Variables[0]).Value;
        }

        public void GetDiscreteMeasurement(string mrid, out States state)
        {
            Command command = new ReadSingleDigital() { Id = mrid };
            Response response = sCADAProxy.ExecuteCommand(command);

            state = ((DigitalVariable)response.Variables[0]).State;
        }
    }
}

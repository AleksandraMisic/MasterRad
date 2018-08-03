using DNP3TCPDriver;
using DNP3TCPDriver.ApplicationLayer;
using DNP3TCPDriver.UserLevel;
using SCADA.RealtimeDatabase;
using SCADA.RealtimeDatabase.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.CommunicationAndControlling.DNP3UserLayer
{
    public class DNP3UserLayerHandler : IDNP3UserLayer
    {
        private DNP3Handler dNP3Handler;
        private DBContext database;

        public DNP3UserLayerHandler(DNP3Handler dNP3Handler, DBContext database)
        {
            if (dNP3Handler == null)
            {
                throw new ArgumentNullException();
            }
            if (database == null)
            {
                throw new ArgumentNullException();
            }

            this.dNP3Handler = dNP3Handler;
            this.database = database;
        }

        public List<byte[]> ReadAllAnalogInputPointsRequest(string rtuName)
        {
            List<UserLevelObject> userLevelObjects = new List<UserLevelObject>();

            foreach (Analog analog in database.GetProcessVariableByTypeAndRTU(VariableTypes.ANALOG, rtuName))
            {
                UserLevelObject userLevelObject = new UserLevelObject()
                {
                    PointType = PointType.ANALOG_INPUT,
                    Variation = Variations.BIT_32_NO_FLAG
                };

                userLevelObjects.Add(userLevelObject);
            }
            
            return dNP3Handler.DNP3ApplicationHandler.PackDown(userLevelObjects, ApplicationFunctionCodes.READ, true, true);
        }
    }
}

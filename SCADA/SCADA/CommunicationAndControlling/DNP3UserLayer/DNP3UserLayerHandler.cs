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
            UserLevelObject userLevelObject = new UserLevelObject()
            {
                PointType = PointType.ANALOG_INPUT,
                Variation = Variations.BIT_32_NO_FLAG,
                RangeFieldPresent = true,
                RangePresent = true,
            };

            int i = 0;
            foreach (Analog analog in database.GetProcessVariableByTypeAndRTU(VariableTypes.ANALOG, rtuName))
            {
                userLevelObject.StopIndex = analog.RelativeAddress;

                if (i++ == 0)
                {
                    userLevelObject.StartIndex = analog.RelativeAddress;
                }
            }

            userLevelObjects.Add(userLevelObject);

            return dNP3Handler.DNP3ApplicationHandler.PackDown(userLevelObjects, ApplicationFunctionCodes.READ, true, true);
        }

        public List<Tuple<string, float>> ReadAllAnalogInputPointsReadResponse(List<UserLevelObject> userLevelObjects, string rtuName)
        {
            List<Tuple<string, float>> retVal = new List<Tuple<string, float>>();

            foreach (UserLevelObject userLevelObject in userLevelObjects)
            {
                if (userLevelObject.FunctionCode == ApplicationFunctionCodes.RESPONSE)
                {
                    switch (userLevelObject.PointType)
                    {
                        case PointType.ANALOG_INPUT:

                            if (userLevelObject.IndicesPresent == false)
                            {
                                if (userLevelObject.RangeFieldPresent == true)
                                {
                                    if (userLevelObject.RangePresent == true)
                                    {
                                        lock (database.LockObject)
                                        {
                                            foreach (Analog analog in database.GetProcessVariableByTypeAndRTU(VariableTypes.ANALOG, rtuName))
                                            {
                                                if (analog.RelativeAddress <= userLevelObject.StartIndex && analog.RelativeAddress >= userLevelObject.StopIndex)
                                                {
                                                    analog.AcqValue = BitConverter.ToInt32(userLevelObject.Values[analog.RelativeAddress], 0);
                                                    retVal.Add(new Tuple<string, float>(analog.Name, analog.AcqValue));
                                                }
                                            }
                                        }
                                    }
                                }
                               
                            }
                            break;
                    }
                }
            }

            return retVal;
        }

        public List<byte[]> ReadAllAnalogInputPointsResponse(List<UserLevelObject> userLevelObjects)
        {
            throw new NotImplementedException();
        }
    }
}

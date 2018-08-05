using DNP3DataPointsModel;
using DNP3Outstation.Model;
using DNP3TCPDriver;
using DNP3TCPDriver.ApplicationLayer;
using DNP3TCPDriver.UserLevel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3Outstation.DNP3UserLayer
{
    public class DNP3UserLayerHandler : IDNP3UserLayer
    {
        private DNP3Handler dNP3Handler;

        public DNP3UserLayerHandler(DNP3Handler dNP3Handler)
        {
            if (dNP3Handler == null)
            {
                throw new ArgumentNullException();
            }

            this.dNP3Handler = dNP3Handler;
        }

        public List<byte[]> ReadAllAnalogInputPointsRequest(string rtuName)
        {
            throw new NotImplementedException();
        }

        public List<byte[]> ReadAllAnalogInputPointsResponse(List<UserLevelObject> userLevelObjects)
        {
            foreach (UserLevelObject userLevelObject in userLevelObjects)
            {
                if (userLevelObject.FunctionCode == ApplicationFunctionCodes.READ)
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
                                        lock (Database.DatabaseLock)
                                        {
                                            foreach (AnalogInputPoint analog in Database.AnalogInputPoints)
                                            {
                                                if (analog.Index <= userLevelObject.StartIndex && analog.Index >= userLevelObject.StopIndex)
                                                {
                                                    userLevelObject.Values.Add(BitConverter.GetBytes(analog.Value));
                                                    userLevelObject.Indices.Add(analog.Index);
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

            return dNP3Handler.DNP3ApplicationHandler.PackDown(userLevelObjects, ApplicationFunctionCodes.RESPONSE, false, false);
        }
    }
}

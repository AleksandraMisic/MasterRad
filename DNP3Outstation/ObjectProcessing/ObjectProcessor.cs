using DNP3DataPointsModel;
using DNP3Driver.ApplicationLayer;
using DNP3Outstation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3Outstation.ObjectProcessing
{
    public class ObjectProcessor
    {
        private DNP3Object DNP3Object;

        public ObjectProcessor(DNP3Object dNP3Object)
        {
            DNP3Object = dNP3Object;
        }

        public byte[] ProcessObject()
        {
            byte[] processedObject = null;

            if(!DNP3Object.IndicesSet && !DNP3Object.ObjectSize)
            {
                switch (DNP3Object.Group)
                {
                    case 30:

                        lock (Database.DatabaseLock)
                        {
                            foreach (AnalogInputPoint analog in Database.AnalogInputPoints)
                            {
                                DNP3Object.Values.Add(analog.Value);
                            }
                        }

                        switch (DNP3Object.Variation)
                        {
                            case 3:

                                break;
                        }
                       break;
                }
            }

            return processedObject;
        }
    }
}

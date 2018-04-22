﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace OMSSCADACommon
{
    [Serializable]
    [DataContract]
    public class ScadaElement
    {
        private DeviceTypes type;

        private string name;

        // to do: in future implementations create classes for analog, discrete, coutner...
      
        //------Propeties for DiscreteMeas
        private List<CommandTypes> validCommands = new List<CommandTypes>();

        private List<States> validStates = new List<States>();

        //------Propertis for AnalogMeas

        ///<summary>Initial value for analog meas</summary>
        private float workPoint; 

        private string unitSymbol;

        [DataMember]
        public DeviceTypes Type
        {
            get { return type; }
            set { type = value; }
        }

        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [DataMember]
        public List<CommandTypes> ValidCommands
        {
            get { return validCommands; }
            set { validCommands = value; }
        }

        [DataMember]
        public List<States> ValidStates
        {
            get { return validStates; }
            set { validStates = value; }
        }

        [DataMember]
        public float WorkPoint
        {
            get { return workPoint; }
            set { workPoint = value; }
        }

        [DataMember]
        public string UnitSymbol
        {
            get { return unitSymbol; }
            set { unitSymbol = value; }
        }
    }
}

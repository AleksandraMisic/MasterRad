using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3DataPointsModel
{
    public class AnalogInputPoint : INotifyPropertyChanged
    {
        private float value;
        private int rawValue;

        private float outValue;
        private string outValueString = "UNKNOWN";
        private int rawOutValue;

        private float masterValue;
        private string masterValueString = "UNKNOWN";
        private int rawMasterValue;

        private string name = "UNKNOWN";
        private string description = "UNKNOWN";
        private string units = "UNKNOWN";

        private bool isFixed;

        public AnalogInputPoint()
        {
            ScaleFactor = 1;
            ScaleOffset = 0;
        }

        public int Index { get; set; }
        public float Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
                RaisePropertyChanged("Value");
            }
        }

        public int RawValue
        {
            get
            {
                return rawValue;
            }
            set
            {
                this.rawValue = value;
                RaisePropertyChanged("RawValue");
            }
        }

        public float OutValue
        {
            get
            {
                return outValue;
            }
            set
            {
                this.outValue = value;
                RaisePropertyChanged("OutValue");
            }
        }

        public string OutValueString
        {
            get
            {
                return outValueString;
            }
            set
            {
                this.outValueString = value;
                RaisePropertyChanged("OutValueString");
            }
        }

        public int RawOutValue
        {
            get
            {
                return rawOutValue;
            }
            set
            {
                this.rawOutValue = value;
                RaisePropertyChanged("RawOutValue");
            }
        }

        public float MasterValue
        {
            get
            {
                return masterValue;
            }
            set
            {
                this.masterValue = value;
                RaisePropertyChanged("MasterValue");
            }
        }

        public string MasterValueString
        {
            get
            {
                return masterValueString;
            }
            set
            {
                this.masterValueString = value;
                RaisePropertyChanged("MasterValueString");
            }
        }

        public int RawMasterValue
        {
            get
            {
                return rawMasterValue;
            }
            set
            {
                this.rawMasterValue = value;
                RaisePropertyChanged("RawMasterValue");
            }
        }

        public bool IsFixed
        {
            get
            {
                return isFixed;
            }
            set
            {
                isFixed = value;
                RaisePropertyChanged("IsFixed");
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                RaisePropertyChanged("Name");
            }
        }

        public int ChangeEventClass { get; set; }
        public int MinIntegerTransmittedValue { get; set; }
        public int MaxIntegerTransmittedValue { get; set; }
        public int ScaleFactor { get; set; }
        public float ScaleOffset { get; set; }

        public string Units
        {
            get
            {
                return units;
            }
            set
            {
                units = value;
                RaisePropertyChanged("Units");
            }
        }

        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
                RaisePropertyChanged("Description");
            }
        }

        public int Resolution { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}

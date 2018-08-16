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
        private int rawOutValue;

        private float masterValue;
        private int rawMasterValue;

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

        public string Name { get; set; }
        public int ChangeEventClass { get; set; }
        public int MinIntegerTransmittedValue { get; set; }
        public int MaxIntegerTransmittedValue { get; set; }
        public int ScaleFactor { get; set; }
        public float ScaleOffset { get; set; }
        public string Units { get; set; }
        public int Resolution { get; set; }
        public string Description { get; set; }

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

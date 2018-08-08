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
        private int value;
        private int rawValue;

        public int Index { get; set; }
        public int Value
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

        public string Name { get; set; }
        public int ChangeEventClass { get; set; }
        public int MinIntegerTransmittedValue { get; set; }
        public int MaxIntegerTransmittedValue { get; set; }
        public int ScaleFactor { get; set; }
        public double ScaleOffset { get; set; }
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

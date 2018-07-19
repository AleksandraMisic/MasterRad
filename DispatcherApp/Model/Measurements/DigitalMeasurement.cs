using FTN.Common;
using OMSSCADACommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DispatcherApp.Model.Measurements
{
    public class DigitalMeasurement : Measurement, INotifyPropertyChanged
    {
        private OMSSCADACommon.States state;

        public new void ReadFromResourceDescription(ResourceDescription rd)
        {
            base.ReadFromResourceDescription(rd);
        }

        public OMSSCADACommon.States State
        {
            get
            {
                return this.state;
            }
            set
            {
                this.state = value;
                RaisePropertyChanged("State");
            }
        }
    }
}

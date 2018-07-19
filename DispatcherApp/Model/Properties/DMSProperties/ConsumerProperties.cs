using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DispatcherApp.Model.Properties.DMSProperties
{
    public class ConsumerProperties : ElementProperties
    {
        private bool call;

        private double scadaSize;
        private double scadaCornerRadius;
        private double fontSize;
        private double scadaTop;
        private double scadaLeft;

        public bool Call
        {
            get
            {
                return call;
            }
            set
            {
                call = value;
                RaisePropertyChanged("Call");
            }
        }

        public double ScadaCornerRadius
        {
            get
            {
                return this.scadaCornerRadius;
            }
            set
            {
                this.scadaCornerRadius = value;
                RaisePropertyChanged("ScadaCornerRadius");
            }
        }

        public double ScadaSize
        {
            get
            {
                return this.scadaSize;
            }
            set
            {
                this.scadaSize = value;
                RaisePropertyChanged("ScadaSize");
            }
        }

        public double FontSize
        {
            get
            {
                return this.fontSize;
            }
            set
            {
                this.fontSize = value;
                RaisePropertyChanged("FontSize");
            }
        }

        public double ScadaTop
        {
            get
            {
                return this.scadaTop;
            }
            set
            {
                this.scadaTop = value;
                RaisePropertyChanged("ScadaTop");
            }
        }

        public double ScadaLeft
        {
            get
            {
                return this.scadaLeft;
            }
            set
            {
                this.scadaLeft = value;
                RaisePropertyChanged("ScadaLeft");
            }
        }
    }
}

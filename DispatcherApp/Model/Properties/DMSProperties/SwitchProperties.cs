using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DispatcherApp.Model.Properties.DMSProperties
{
    public class SwitchProperties : ElementProperties
    {
        private bool incident;
        private bool crewSent;
        private bool canCommand;
        private bool isCandidate;
        private long parentGid;
        private ElementProperties parent;

        private double cornerRadius;
        private double scadaSize;
        private double scadaCornerRadius;
        private double fontSize;
        private double scadaTop;
        private double scadaLeft;

        public bool Incident
        {
            get
            {
                return this.incident;
            }
            set
            {
                this.incident = value;
                RaisePropertyChanged("Incident");
            }
        }

        public bool CrewSent
        {
            get
            {
                return this.crewSent;
            }
            set
            {
                this.crewSent = value;
                RaisePropertyChanged("CrewSent");
            }
        }

        public bool IsCandidate
        {
            get
            {
                return this.isCandidate;
            }
            set
            {
                this.isCandidate = value;
                RaisePropertyChanged("IsCandidate");
            }
        }

        public bool CanCommand
        {
            get
            {
                return this.canCommand;
            }
            set
            {
                this.canCommand = value;
                RaisePropertyChanged("CanCommand");
            }
        }

        public long ParentGid
        {
            get
            {
                return this.parentGid;
            }
            set
            {
                this.parentGid = value;
                RaisePropertyChanged("ParentGid");
            }
        }

        public ElementProperties Parent
        {
            get
            {
                return this.parent;
            }
            set
            {
                this.parent = value;
                RaisePropertyChanged("Parent");
            }
        }

        public double CornerRadius
        {
            get
            {
                return this.cornerRadius;
            }
            set
            {
                this.cornerRadius = value;
                RaisePropertyChanged("CornerRadius");
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

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
    }
}

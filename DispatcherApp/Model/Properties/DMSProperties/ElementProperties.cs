using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DispatcherApp.Model.Properties.DMSProperties
{
    public class ElementProperties : INotifyPropertyChanged
    {
        private long gID;
        private bool isEnergized;
        private bool isUnderScada;

        public long GID
        {
            get
            {
                return this.gID;
            }
            set
            {
                this.gID = value;
                RaisePropertyChanged("GID");
            }
        }

        public bool IsEnergized
        {
            get
            {
                return this.isEnergized;
            }
            set
            {
                this.isEnergized = value;
                RaisePropertyChanged("IsEnergized");
            }
        }

        public bool IsUnderScada
        {
            get
            {
                return this.isUnderScada;
            }
            set
            {
                this.isUnderScada = value;
                RaisePropertyChanged("IsUnderScada");
            }
        }

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

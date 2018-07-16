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
    }
}

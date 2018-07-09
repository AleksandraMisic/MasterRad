using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MITM_UI.Model
{
    public class Attack : INotifyPropertyChanged
    {
        private string target1;
        private string target2;
        private AttackMethod method;
        public CancellationTokenSource TokenSource = new CancellationTokenSource();

        public string Target1
        {
            get
            {
                return target1;
            }
            set
            {
                target1 = value;
                RaisePropertyChanged("Target1");
            }
        }

        public string Target2
        {
            get
            {
                return target2;
            }
            set
            {
                target2 = value;
                RaisePropertyChanged("Target2");
            }
        }

        public AttackMethod Method
        {
            get
            {
                return method;
            }
            set
            {
                method = value;
                RaisePropertyChanged("Method");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}

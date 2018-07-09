using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITM_UI.Model.GlobalInfo
{
    public class GlobalActiveAttacks
    {
        public static ObservableCollection<Attack> ActiveAttacks { get; set; }
    }
}

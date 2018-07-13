﻿using MITM_Common.MITM_Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIShell.ViewModel;

namespace MITM_UI.Model.GlobalInfo
{
    public class Database
    {
        public static GlobalConnectionInfo GlobalConnectionInfo { get; set; }

        public static Dictionary<ViewModelType, SingleShellFillerViewModel> ViewModels;

        static Database()
        {
            GlobalConnectionInfo = new GlobalConnectionInfo();
            ViewModels = new Dictionary<ViewModelType, SingleShellFillerViewModel>();
        }
    }
}
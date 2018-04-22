﻿using DispatcherApp.Model;
using DispatcherApp.ViewModel.ShellFillerViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DispatcherApp.ViewModel.ShellFillerModelViews
{
    public class PropertiesModelView : ShellFillerViewModel
    {
        private static bool isOpen;
        private static ShellPosition position;

        public override bool IsOpen
        {
            get { return isOpen; }
            set { isOpen = value; }
        }

        public static ShellPosition Position
        {
            get { return position; }
            set { position = value; }
        }

        static PropertiesModelView()
        {
            Position = ShellPosition.RIGHT;
        }

        public PropertiesModelView()
        {

        }
    }
}

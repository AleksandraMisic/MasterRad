using DNP3DataPointsModel;
using DNP3Outstation.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UIShell.Model;
using UIShell.ViewModel;

namespace DNP3Outstation.ViewModel
{
    public class DataPointsViewModel : SingleShellFillerViewModel
    {
        private static bool isOpen;
        private static ShellPosition position;

        public ObservableCollection<AnalogInputPoint> AnalogInputPoints { get; set; }

        public static ShellPosition Position
        {
            get { return position; }
            set { position = value; }
        }

        public DataPointsViewModel()
        {
            Position = ShellPosition.CENTER;
            AnalogInputPoints = new ObservableCollection<AnalogInputPoint>();

            foreach (AnalogInputPoint analog in Database.AnalogInputPoints)
            {
                AnalogInputPoints.Add(analog);
            }
        }

        public override bool IsOpen
        {
            get { return isOpen; }
            set { isOpen = value; }
        }

        void AnalogPointsSimulation()
        {
            while (true)
            {
                lock (Database.DatabaseLock)
                {
                    foreach (AnalogInputPoint analog in Database.AnalogInputPoints)
                    {
                        var an = AnalogInputPoints.Where(a => a.Name == analog.Name).FirstOrDefault();
                        an.Value = analog.Value;
                    }
                }

                Thread.Sleep(1000);
            }
        }
    }
}

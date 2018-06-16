using DMSCommon.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DispatcherApp.Model.DrawingAlgorithms.Schematic.RadialNetwork.Placing
{
    public abstract class DrawElement
    {
        public abstract void Draw(ObservableCollection<UIElement> list, double cellHeight, double cellWidth, double currentWidth, Point point1, Point point2, Element element);
    }
}

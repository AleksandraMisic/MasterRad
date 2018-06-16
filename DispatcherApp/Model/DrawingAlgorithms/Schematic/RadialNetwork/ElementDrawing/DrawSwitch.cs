using DispatcherApp.Model.DrawingAlgorithms.Schematic.RadialNetwork.Placing;
using DispatcherApp.View.CustomControls;
using DMSCommon.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DispatcherApp.Model.DrawingAlgorithms.Schematic.RadialNetwork.ElementDrawing
{
    public class DrawSwitch : DrawElement
    {
        public override void Draw(ObservableCollection<UIElement> list, double cellHeight, double cellWidth, double currentWidth, Point point1, Point point2, Element element)
        {
            SwitchControl switchControl = new SwitchControl(element, currentWidth);

            Canvas.SetLeft(switchControl, point2.X - (currentWidth) / 2 - currentWidth - 2);
            Canvas.SetTop(switchControl, point2.Y - (cellHeight / 3) - (currentWidth) / 2);
            Canvas.SetZIndex(switchControl, 5);

            //switchControl.Button.Command = PropertiesCommand;
            switchControl.Button.CommandParameter = element.ElementGID;
            switchControl.ButtonCanvas.ToolTip = element.MRID;

            list.Add(switchControl);
        }
    }
}

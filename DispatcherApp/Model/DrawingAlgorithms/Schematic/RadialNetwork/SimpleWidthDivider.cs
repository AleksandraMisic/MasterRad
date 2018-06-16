using DispatcherApp.View.CustomControls;
using DispatcherApp.View.CustomControls.NetworkElementsControls;
using DMSCommon.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DispatcherApp.Model.DrawingAlgorithms.Schematic.RadialNetwork
{
    public class SimpleWidthDivider
    {
        private double canvasHeight;
        private double canvasWidth;
        private double cellHeight;
        private List<Element> elementList;
        private ObservableCollection<UIElement> result;

        private FrameworkElement frameworkElement;

        public SimpleWidthDivider()
        {
            frameworkElement = new FrameworkElement();
            canvasHeight = (double)frameworkElement.FindResource("NetworkCanvasHeight");
            canvasWidth = (double)frameworkElement.FindResource("NetworkCanvasWidth");
        }

        public void DrawNetwork(List<Element> list, int networkDepth, ObservableCollection<UIElement> result)
        {
            this.result = result;
            this.elementList = list;
            this.cellHeight = canvasHeight / (networkDepth + 2);

            for (int i = 1; i < networkDepth + 2; i++)
            {
                PlaceGridLines(i);
            }

            Branch branch = list[0] as Branch;

            if (branch != null)
            {
                Element element;
                element = this.elementList.Where(el => el.ElementGID == branch.End2).FirstOrDefault();

                if (element != null)
                {
                    Node node = element as Node;

                    if (node != null)
                    {
                        DrawRecursion(node, 0, 1, this.canvasWidth);
                    }
                }
            }
        }

        private void DrawRecursion(Node node, double offset, int y, double cellWidth)
        {
            PlaceNode(node, offset, y, cellWidth);

            Point point1 = new Point(offset + cellWidth / 2, (y++) * cellHeight);

            cellWidth = cellWidth / node.Children.Count;

            for (int i = 0; i < node.Children.Count; i++)
            {
                double newOffset = offset + i * cellWidth;

                Element element;
                element = this.elementList.Where(el => el.ElementGID == node.Children[i]).FirstOrDefault();

                Branch branch = element as Branch;
                if (branch != null)
                {
                    PlaceBranch(point1, newOffset, i, y, cellWidth, branch);

                    element = this.elementList.Where(el => el.ElementGID == branch.End2).FirstOrDefault();

                    if (element != null)
                    {
                        DrawRecursion(element as Node, newOffset, y, cellWidth);
                    }
                }
            }
        }

        private void PlaceGridLines(int i)
        {
            Line line = new Line();
            line.Stroke = (SolidColorBrush)frameworkElement.FindResource("MediumDarkColor");
            line.StrokeThickness = 0.3;
            line.X1 = 0;
            line.X2 = canvasWidth;
            line.Y1 = i * cellHeight;
            line.Y2 = i * cellHeight;

            this.result.Add(line);
        }

        private void PlaceNode(Node node, double offset, int y, double cellWidth)
        {
            NodeControl nodeControl = new NodeControl(3, 3);
            Canvas.SetLeft(nodeControl, offset + cellWidth / 2 - nodeControl.Width / 2);
            Canvas.SetTop(nodeControl, y * cellHeight - nodeControl.Height / 2);
            Canvas.SetZIndex(nodeControl, 5);

            //node.Command = PropertiesCommand;
            nodeControl.CommandParameter = node.ElementGID;
            nodeControl.ToolTip = node.MRID;

            this.result.Add(nodeControl);
        }

        private void PlaceBranch(Point point1, double offset, int x, int y, double cellWidth, Branch branch)
        {
            Point point2 = new Point(offset + cellWidth / 2, y * cellHeight);
            Point point3 = new Point(point2.X, point2.Y - 2 * (cellHeight / 3));

            if (branch is Source)
            {
                //PlaceSource(branch.ElementGID, branch.MRID);
            }
            else if (branch is Switch)
            {
                PlaceSwitch(branch as Switch, offset, y, cellWidth);
            }
            else if (branch is ACLine)
            {
                PlaceACLineSegment(branch as ACLine, offset, y, cellWidth);
            }

            if (branch is Switch)
            {
                Point point4 = new Point(point2.X, point2.Y - cellHeight / 3);

                Polyline polyline1 = new Polyline();
                Polyline polyline2 = new Polyline();
                polyline1.Points.Add(point4);
                polyline1.Points.Add(point2);

                polyline1.StrokeThickness = 0.5;
                polyline1.Stroke = Brushes.Red;
                Canvas.SetZIndex(polyline1, 0);
                //    polyline1.DataContext = prop;

                polyline2.Points.Add(point1);
                polyline2.Points.Add(point3);
                polyline2.Points.Add(point4);

                polyline2.StrokeThickness = 0.5;
                polyline2.Stroke = Brushes.Yellow;
                Canvas.SetZIndex(polyline2, 0);

                this.result.Add(polyline1);
                this.result.Add(polyline2);
                //    polyline2.DataContext = prop;

                //    Style style1 = new Style();
                //    Style style2 = new Style();
                //    Setter setter1 = new Setter() { Property = Polyline.StrokeProperty, Value = (SolidColorBrush)frameworkElement.FindResource("SwitchColorClosed") };

                //    DataTrigger trigger1 = new DataTrigger() { Binding = new Binding("IsEnergized"), Value = false };
                //    Setter setter2 = new Setter()
                //    {
                //        Property = Polyline.StrokeProperty,
                //        Value = (SolidColorBrush)frameworkElement.FindResource("SwitchColorClosedDeenergized")
                //    };

                //    trigger1.Setters.Add(setter2);

                //    DataTrigger trigger2 = new DataTrigger() { Binding = new Binding("Parent.IsEnergized"), Value = false };

                //    trigger2.Setters.Add(setter2);

                //    style1.Setters.Add(setter1);
                //    style1.Triggers.Add(trigger1);

                //    style2.Setters.Add(setter1);
                //    style2.Triggers.Add(trigger2);

                //    polyline1.Style = style1;
                //    polyline2.Style = style2;

                //    mainCanvas.Children.Add(polyline1);
                //    mainCanvas.Children.Add(polyline2);
            }
            else
            {
                Polyline polyline1 = new Polyline();
                polyline1.Points.Add(point1);
                polyline1.Points.Add(point3);
                polyline1.Points.Add(point2);

                polyline1.StrokeThickness = 0.5;
                Canvas.SetZIndex(polyline1, 0);
                polyline1.Stroke = Brushes.Yellow;

                this.result.Add(polyline1);
                //polyline1.DataContext = prop;

                //    Style style1 = new Style();
                //    Setter setter1 = new Setter() { Property = Polyline.StrokeProperty, Value = (SolidColorBrush)frameworkElement.FindResource("SwitchColorClosed") };

                //    DataTrigger trigger1 = new DataTrigger() { Binding = new Binding("Parent.IsEnergized"), Value = false };
                //    Setter setter2 = new Setter()
                //    {
                //        Property = Polyline.StrokeProperty,
                //        Value = (SolidColorBrush)frameworkElement.FindResource("SwitchColorClosedDeenergized")
                //    };

                //    trigger1.Setters.Add(setter2);

                //    style1.Setters.Add(setter1);
                //    style1.Triggers.Add(trigger1);

                //    polyline1.Style = style1;

                //    mainCanvas.Children.Add(polyline1);
            }
        }

        private void PlaceSource()
        {

        }

        private void PlaceSwitch(Switch breaker, double offset, int y, double cellWidth)
        {
            SwitchControl switchControl = new SwitchControl(breaker, 10);

            Canvas.SetLeft(switchControl, offset + cellWidth / 2 - (2 + 10 + 10 / 2));
            Canvas.SetTop(switchControl, y * cellHeight - cellHeight / 3 - 10 / 2);
            Panel.SetZIndex(switchControl, 5);

            //switchControl.Button.Command = PropertiesCommand;
            switchControl.Button.CommandParameter = breaker.ElementGID;
            switchControl.ButtonCanvas.ToolTip = breaker.MRID;

            result.Add(switchControl);
        }

        private void PlaceACLineSegment(ACLine acLine, double offset, int y, double cellWidth)
        {
            ACLineControl acLineControl = new ACLineControl(acLine, 3, 10);

            Canvas.SetLeft(acLineControl, offset + cellWidth / 2 - 1.75);
            Canvas.SetTop(acLineControl, y * cellHeight - cellHeight / 3 - 10 / 2);
            Panel.SetZIndex(acLineControl, 5);

            //switchControl.Button.Command = PropertiesCommand;
            acLineControl.CommandParameter = acLine.ElementGID;
            acLineControl.ToolTip = acLine.MRID;

            result.Add(acLineControl);
        }
    }
}

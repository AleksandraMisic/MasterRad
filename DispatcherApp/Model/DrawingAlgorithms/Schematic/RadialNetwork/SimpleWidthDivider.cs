using DispatcherApp.Model.Properties.DMSProperties;
using DispatcherApp.View.CustomControls;
using DispatcherApp.View.CustomControls.NetworkElementsControls;
using DispatcherApp.ViewModel.ShellFillerModelViews;
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
        private Dictionary<long, ElementProperties> properties;
        private ObservableCollection<UIElement> result;

        private double currentHeight = 8;
        private double startHeight = 8;

        private FrameworkElement frameworkElement;

        public SimpleWidthDivider()
        {
            frameworkElement = new FrameworkElement();
            canvasHeight = (double)frameworkElement.FindResource("NetworkCanvasHeight");
            canvasWidth = (double)frameworkElement.FindResource("NetworkCanvasWidth");

            this.elementList = new List<Element>();
            this.properties = new Dictionary<long, ElementProperties>();
        }

        public SimpleWidthDivider(List<Element> elementList, Dictionary<long, ElementProperties> properties)
        {
            frameworkElement = new FrameworkElement();
            canvasHeight = (double)frameworkElement.FindResource("NetworkCanvasHeight");
            canvasWidth = (double)frameworkElement.FindResource("NetworkCanvasWidth");

            this.elementList = elementList;
            this.properties = properties;
        }

        public void DrawNetwork(int networkDepth, ObservableCollection<UIElement> result)
        {
            this.result = result;
            this.cellHeight = canvasHeight / (networkDepth + 2);

            for (int i = 1; i < networkDepth + 2; i++)
            {
                PlaceGridLines(i);
            }

            Branch branch = elementList[0] as Branch;
            PlaceBranch(new Point(this.canvasWidth/2, 0), 0, 0, 1, this.canvasWidth, branch);

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
            NodeUserControl nodeControl = new NodeUserControl();

            Canvas.SetLeft(nodeControl, offset + cellWidth / 2 - nodeControl.Button.Width / 2);
            Canvas.SetTop(nodeControl, y * cellHeight - nodeControl.Button.Height / 2);
            Canvas.SetZIndex(nodeControl, 5);

            nodeControl.Button.Command = NetworkViewViewModel.OpenPropertiesCommand;
            nodeControl.Button.CommandParameter = node.ElementGID;
            nodeControl.ToolTip = node.MRID;

            this.result.Add(nodeControl);
        }

        private void PlaceBranch(Point point1, double offset, int x, int y, double cellWidth, Branch branch)
        {
            Point point2 = new Point(offset + cellWidth / 2, y * cellHeight);
            Point point3 = new Point(point2.X, point2.Y - 2 * (cellHeight / 3));

            if (branch is Source)
            {
                PlaceSource(branch as Source, offset, y - 1, cellWidth);
            }
            else if (branch is Switch)
            {
                PlaceSwitch(branch as Switch, offset, y, cellWidth);
            }
            else if (branch is ACLine)
            {
                PlaceACLineSegment(branch as ACLine, offset, y, cellWidth);
            }
            else if (branch is Consumer)
            {
                PlaceConsumer(branch as Consumer, offset, y, cellWidth);
            }

            if (branch is Switch)
            {
                Point point4 = new Point(point2.X, point2.Y - cellHeight / 3);

                SwitchLineControl switchLineControl = new SwitchLineControl();

                properties.TryGetValue(branch.ElementGID, out ElementProperties switchProperties);

                if (switchProperties != null)
                {
                    switchLineControl.Polyline1.DataContext = switchProperties as SwitchProperties;
                    switchLineControl.Polyline2.DataContext = switchProperties as SwitchProperties;
                }

                switchLineControl.Polyline1.Points.Add(point1);
                switchLineControl.Polyline1.Points.Add(point3);
                switchLineControl.Polyline1.Points.Add(point4);

                switchLineControl.Polyline2.Points.Add(point4);
                switchLineControl.Polyline2.Points.Add(point2);

                Canvas.SetZIndex(switchLineControl, 0);

                this.result.Add(switchLineControl);
            }
            else
            {
                RegularLineControl regularLineControl = new RegularLineControl();
                properties.TryGetValue(branch.ElementGID, out ElementProperties branchProperties);

                if (branch is Consumer)
                {
                    point2.Y = y * cellHeight - cellHeight / 5;

                    if (branchProperties != null)
                    {
                        regularLineControl.Polyline1.DataContext = branchProperties as ConsumerProperties;
                    }
                }
                else if (branch is ACLine)
                {
                    if (branchProperties != null)
                    {
                        regularLineControl.Polyline1.DataContext = branchProperties as ACLineProperties;
                    }
                }
                else if (branch is Source)
                {
                    point1.Y += 3;
                    point2.X = point1.X;
                    point3.X = point1.X;

                    if (branchProperties != null)
                    {
                        regularLineControl.Polyline1.DataContext = branchProperties as SourceProperties;
                    }
                }

                regularLineControl.Polyline1.Points.Add(point1);
                regularLineControl.Polyline1.Points.Add(point3);
                regularLineControl.Polyline1.Points.Add(point2);

                Canvas.SetZIndex(regularLineControl, 0);

                this.result.Add(regularLineControl);
            }
        }

        private void PlaceSource(Source source, double offset, int y, double cellWidth)
        {
            SourceUserControl sourceControl = new SourceUserControl();

            double var = 80 * cellWidth / 100;

            if (var > startHeight)
            {
                currentHeight = startHeight;
            }
            else
            {
                currentHeight = var;
            }

            if (properties.TryGetValue(source.ElementGID, out ElementProperties elementProperties))
            {
                SourceProperties sourceProperties = elementProperties as SourceProperties;

                sourceControl.DataContext = sourceProperties;
            }

            sourceControl.Button.Width = currentHeight;
            sourceControl.Button.Height = currentHeight;

            Canvas.SetLeft(sourceControl, offset + cellWidth / 2 - currentHeight / 2);
            Canvas.SetTop(sourceControl, y * cellHeight - cellHeight / 5 + currentHeight);
            Panel.SetZIndex(sourceControl, 5);

            sourceControl.Button.Command = NetworkViewViewModel.OpenPropertiesCommand;
            sourceControl.Button.CommandParameter = source.ElementGID;
            sourceControl.ToolTip = source.MRID;

            result.Add(sourceControl);
        }

        private void PlaceConsumer(Consumer consumer, double offset, int y, double cellWidth)
        {
            ConsumerUserControl consumerControl = new ConsumerUserControl();

            double var = 80 * cellWidth / 100;

            if (var > startHeight)
            {
                currentHeight = startHeight;
            }
            else
            {
                currentHeight = var;
            }

            if (properties.TryGetValue(consumer.ElementGID, out ElementProperties elementProperties))
            {
                ConsumerProperties consumerProperties = elementProperties as ConsumerProperties;

                consumerControl.DataContext = consumerProperties;

                consumerProperties.ScadaCornerRadius = 2 * currentHeight / 6;
                consumerProperties.ScadaSize = 2 * currentHeight / 3;
                consumerProperties.FontSize = 90 * consumerProperties.ScadaSize / 100;

                consumerProperties.ScadaTop = -(2 * currentHeight / 3 / 3);
                consumerProperties.ScadaLeft = -(2 * currentHeight / 3 / 3);
            }

            consumerControl.Button.Width = currentHeight;
            consumerControl.Button.Height = currentHeight;

            Canvas.SetLeft(consumerControl, offset + cellWidth / 2 - currentHeight / 2);
            Canvas.SetTop(consumerControl, y * cellHeight - cellHeight / 5 - currentHeight / 2);
            Panel.SetZIndex(consumerControl, 5);

            consumerControl.Button.Command = NetworkViewViewModel.OpenPropertiesCommand;
            consumerControl.Button.CommandParameter = consumer.ElementGID;
            consumerControl.ToolTip = consumer.MRID;

            result.Add(consumerControl);
        }

        private void PlaceSwitch(Switch breaker, double offset, int y, double cellWidth)
        {
            SwitchUserControl switchControl = new SwitchUserControl();
            double var = 80 * cellWidth / 100;

            if (var > startHeight)
            {
                currentHeight = startHeight;
            }
            else
            {
                currentHeight = var;
            }

            if (properties.TryGetValue(breaker.ElementGID, out ElementProperties elementProperties))
            {
                SwitchProperties switchProperties = elementProperties as SwitchProperties;

                switchControl.DataContext = switchProperties;

                switchProperties.CornerRadius = currentHeight/10;
                switchProperties.ScadaCornerRadius = 2 * currentHeight / 6;
                switchProperties.ScadaSize = 2 * currentHeight / 3;
                switchProperties.FontSize = 90 * switchProperties.ScadaSize / 100;

                switchProperties.ScadaTop = -(2 * currentHeight / 3 / 3);
                switchProperties.ScadaLeft = -(2 * currentHeight / 3 / 3);
            }

            switchControl.IncidentColumn.Width = new GridLength(currentHeight);
            switchControl.MainColumn.Width = new GridLength(currentHeight);
            switchControl.CrewColumn.Width = new GridLength(currentHeight);
            switchControl.MainRow.Height = new GridLength(currentHeight);
            switchControl.Gap1.Width = new GridLength(currentHeight/5);
            switchControl.Gap2.Width = new GridLength(currentHeight/5);

            Canvas.SetLeft(switchControl, offset + cellWidth / 2 - (2 + currentHeight + currentHeight / 2));
            Canvas.SetTop(switchControl, y * cellHeight - cellHeight / 3 - currentHeight / 2);
            Panel.SetZIndex(switchControl, 5);

            switchControl.MainButton.Command = NetworkViewViewModel.OpenPropertiesCommand;
            switchControl.MainButton.CommandParameter = breaker.ElementGID;
            switchControl.MainButton.ToolTip = breaker.MRID;

            result.Add(switchControl);
        }

        private void PlaceACLineSegment(ACLine acLine, double offset, int y, double cellWidth)
        {
            ACLineUserControl acLineControl = new ACLineUserControl();

            if (properties.TryGetValue(acLine.ElementGID, out ElementProperties branchProperties))
            {
                acLineControl.DataContext = branchProperties as ACLineProperties;
            }

            double var = 80 * cellWidth / 100;

            if (var > startHeight)
            {
                currentHeight = startHeight;
            }
            else
            {
                currentHeight = var;
            }

            acLineControl.Button.Height = currentHeight;
            acLineControl.Button.Width = currentHeight/3;

            Canvas.SetLeft(acLineControl, offset + cellWidth / 2 - currentHeight/3/2);
            Canvas.SetTop(acLineControl, y * cellHeight - cellHeight / 3 - currentHeight / 2);
            Panel.SetZIndex(acLineControl, 5);

            acLineControl.Button.Command = NetworkViewViewModel.OpenPropertiesCommand;
            acLineControl.Button.CommandParameter = acLine.ElementGID;
            acLineControl.ToolTip = acLine.MRID;

            result.Add(acLineControl);
        }
    }
}

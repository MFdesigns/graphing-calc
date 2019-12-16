using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GraphingCalc
{
    public partial class MainWindow : Window
    {

        private readonly static int UnitSize = 10;
        private static double CanvasWidth;
        private static double CanvasHeight;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void WindowLoaded(object sender, RoutedEventArgs e)
        {
            RenderGrid();

            // DEBUG
            List<Point> points = CalcGraphPoints();
            RenderGraph(points);
        }

        private void CalcButtonClick(object sender, RoutedEventArgs e)
        {
            RenderGrid();
            List<Point> points = CalcGraphPoints();
            RenderGraph(points);
        }

        private Point GetVertexPoint(int a, int b, int c)
        {
            Point vertex = new Point();

            int u = (-b / (2 * a));
            int v = (((4 * a * c) - (b * b)) / (4 * a));

            return vertex;
        }

        private List<Point> CalcGraphPoints()
        {
            List<Point> points = new List<Point>();

            // TODO: Try / Catch
            int a = Int32.Parse(inputVarA.Text);
            int b = Int32.Parse(inputVarB.Text);
            int c = Int32.Parse(inputVarC.Text);

            Point vertex = GetVertexPoint(a, b, c);

            for (int x = (int)vertex.X - 5; x < (int)vertex.X + 5; x++)
            {
                int y = (int)(a * Math.Pow(x, 2) + b * x + c);

                int viewportX = (int)(CanvasWidth / 2 + x * (UnitSize * 5));
                int viewportY =  (int)(CanvasHeight / 2 - y * (UnitSize * 5));
                Point pnt = new Point(viewportX, viewportY);
                points.Add(pnt);
            }

            return points;
        }
        
        private void RenderGraph(List<Point> points)
        {
            SolidColorBrush graphBrush = new SolidColorBrush(Colors.Blue);

            Polygon graph = new Polygon();
            graph.Stroke = graphBrush;
            graph.Points = new PointCollection(points);

            mainCanvas.Children.Add(graph);
        }

        private void RenderGrid()
        {
            mainCanvas.Children.Clear();

            CanvasWidth = mainCanvas.ActualWidth;
            CanvasHeight = mainCanvas.ActualHeight;
            double canvasMiddleX = CanvasWidth / 2;
            double canvasMiddleY = CanvasHeight / 2;

            int gridCountX = (int)(CanvasWidth / UnitSize);
            int gridCountY = (int)(CanvasHeight / UnitSize);

            SolidColorBrush gridBrush = new SolidColorBrush(Color.FromArgb(255, 222, 222, 222));
            SolidColorBrush gridBrushAccent = new SolidColorBrush(Color.FromArgb(255, 194, 194, 194));
            SolidColorBrush crosshairBrush = new SolidColorBrush(Colors.Black);

            // Vertical grid lines
            for (int x = 1; x < (gridCountX / 2) + 1; x++)
            {
                int strokeThickenss = 1;
                SolidColorBrush colorBrush = gridBrush;
                if (x % 5 == 0)
                {
                    strokeThickenss = 2;
                    colorBrush = gridBrushAccent;
                }

                Line linePos = new Line { 
                    X1 = canvasMiddleX + UnitSize * x,
                    X2 = canvasMiddleX + UnitSize * x,
                    Y1 = 0,
                    Y2 = CanvasHeight,
                    Stroke = colorBrush,
                    StrokeThickness = strokeThickenss,
                };

                Line lineNeg = new Line {
                    X1 = canvasMiddleX - UnitSize * x,
                    X2 = canvasMiddleX - UnitSize * x,
                    Y1 = 0,
                    Y2 = CanvasHeight,
                    Stroke = colorBrush,
                    StrokeThickness = strokeThickenss,
                };

                mainCanvas.Children.Add(linePos);
                mainCanvas.Children.Add(lineNeg);
            }

            // Horizontal grid lines
            for (int y = 1; y < (gridCountY / 2) + 1; y++)
            {
                int strokeThickenss = 1;
                SolidColorBrush colorBrush = gridBrush;
                if (y % 5 == 0)
                {
                    strokeThickenss = 2;
                    colorBrush = gridBrushAccent;
                }

                Line linePos = new Line {
                    X1 = 0,
                    X2 = CanvasWidth,
                    Y1 = canvasMiddleY + UnitSize * y,
                    Y2 = canvasMiddleY + UnitSize * y,
                    Stroke = colorBrush,
                    StrokeThickness = strokeThickenss,
                };
                
                Line lineNeg = new Line {
                    X1 = 0,
                    X2 = CanvasWidth,
                    Y1 = canvasMiddleY - UnitSize * y,
                    Y2 = canvasMiddleY - UnitSize * y,
                    Stroke = colorBrush,
                    StrokeThickness = strokeThickenss,
                };

                mainCanvas.Children.Add(linePos);
                mainCanvas.Children.Add(lineNeg);
            }

            // Crosshair
            Line crosshairX = new Line {
                X1 = 0,
                X2 = CanvasWidth,
                Y1 = (int)(CanvasHeight / 2),
                Y2 = (int)(CanvasHeight / 2),
                Stroke = crosshairBrush,
            };
            
            mainCanvas.Children.Add(crosshairX);

            Line crosshairY = new Line {
                X1 = (int)(CanvasWidth / 2),
                X2 = (int)(CanvasWidth / 2),
                Y1 = 0,
                Y2 = CanvasHeight,
                Stroke = crosshairBrush,
            };
            
            mainCanvas.Children.Add(crosshairY);
        }
    }
}

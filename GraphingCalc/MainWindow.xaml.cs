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
using System.Windows.Interop;

namespace GraphingCalc
{
    public partial class MainWindow : Window
    {
        private static double Scale = 100.0;

        private static double CanvasWidth;
        private static double CanvasHeight;
        private static int OffsetX;
        private static int OffsetY;

        private static readonly List<Graph> Formulas = new List<Graph>();

        private static Random Rand;

        private const int WmExitSizeMove = 0x232;

        public MainWindow()
        {
            Rand = new Random();

            InitializeComponent();
        }

        private IntPtr HwndMessageHook(IntPtr wnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WmExitSizeMove:
                    RenderCanvas();
                    handled = true;
                    break;
            }
            return IntPtr.Zero;
        }

        private Point GetVertexPoint(int a, int b, int c)
        {
            int u = (-b / (2 * a));
            int v = (((4 * a * c) - (b * b)) / (4 * a));

            Point vertex = new Point(u, v);
            return vertex;
        }

        private void AddGraph()
        {
            // Create random color
            byte r = (byte)Rand.Next(0, 256);
            byte g = (byte)Rand.Next(0, 256);
            byte b = (byte)Rand.Next(0, 256);
            Color graphColor = Color.FromRgb(r, g, b);

            SolidColorBrush borderBrush = new SolidColorBrush(Color.FromRgb(220, 220, 220));
            SolidColorBrush colorBrush = new SolidColorBrush(graphColor);
            int fontSize = 14;

            // TextBox maring
            Thickness padding = new Thickness(3, 10, 3, 10);

            // Border container
            Border border = new Border
            {
                BorderThickness = new Thickness(0, 1, 0, 1),
                BorderBrush = borderBrush
            };

            // Panel container
            StackPanel cont = new StackPanel
            {
                Margin = new Thickness(0, 5, 0, 5),
                Orientation = Orientation.Horizontal
            };
            border.Child = cont;

            // Color display
            Rectangle colorPanel = new Rectangle
            {
                Fill = colorBrush,
                Width = 10
            };
            cont.Children.Add(colorPanel);

            // a input
            TextBox aInput = new TextBox
            {
                Text = "1",
                Width = 30,
                Margin = padding
            };
            aInput.TextChanged += FormulaInputChanged;
            cont.Children.Add(aInput);

            // "x^2" label
            TextBlock labelXPower = new TextBlock
            {
                Margin = padding,
                Text = "x",
                FontWeight = FontWeights.Bold,
                FontSize = fontSize
            };

            Run powerLabel = new Run();
            powerLabel.Typography.Variants = FontVariants.Superscript;
            powerLabel.Text = "2";
            labelXPower.Inlines.Add(powerLabel);

            // "+" label
            TextBlock labelPlus = new TextBlock
            {
                Text = "+",
                FontSize = fontSize,
                FontWeight = FontWeights.Bold
            };
            labelXPower.Inlines.Add(labelPlus);
            cont.Children.Add(labelXPower);

            // b input
            TextBox bInput = new TextBox
            {
                Text = "0",
                Width = 30,
                Margin = padding
            };
            bInput.TextChanged += FormulaInputChanged;
            cont.Children.Add(bInput);

            // "x +" label
            TextBlock xPlus = new TextBlock
            {
                Text = "x +",
                Margin = padding,
                FontSize = fontSize,
                FontWeight = FontWeights.Bold
            };
            cont.Children.Add(xPlus);

            // c input
            TextBox cInput = new TextBox
            {
                Text = "0",
                Width = 30,
                Margin = padding
            };
            cInput.TextChanged += FormulaInputChanged;
            cont.Children.Add(cInput);

            Graph frm = new Graph(aInput, bInput, cInput, graphColor);
            Formulas.Add(frm);

            // Add to formula panel
            FormulaPanel.Children.Add(border);

            // Render canvas
            RenderCanvas();
        }

        private List<Point> CalcGraphPoints(int formulaIndex)
        {
            List<Point> points = new List<Point>();

            bool parseA = Int32.TryParse(Formulas[formulaIndex].InputA.Text, out int a);
            bool parseB = Int32.TryParse(Formulas[formulaIndex].InputB.Text, out int b);
            bool parseC = Int32.TryParse(Formulas[formulaIndex].InputC.Text, out int c);

            if (parseA && parseB && parseC)
            {
                RenderVertex(a, b, c, formulaIndex);

                double step = 1 / Scale;
                for (double x = -(CanvasWidth / 2); x < CanvasWidth / 2; x += step)
                {
                    double y = (a * Math.Pow(x, 2) + b * x + c) * -1; // -1 to flip cordinate system

                    if (y > -(CanvasHeight / 2) && y < CanvasHeight / 2)
                    {
                        double viewportX = OffsetX + x * Scale;
                        double viewportY = OffsetY + y * Scale;
                        Point pnt = new Point(viewportX, viewportY);
                        points.Add(pnt);
                    };
                }
            }

            return points;
        }

        private void RenderVertex(int a, int b, int c, int index)
        {

            Point vertex = GetVertexPoint(a, b, c);

            Color color = Formulas[index].Color;

            StackPanel cont = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            Ellipse vertexColor = new Ellipse
            {
                Width = 15,
                Height = 15,
                Fill = new SolidColorBrush(color),
                Margin = new Thickness(0, 0, 10, 0)
            };
            cont.Children.Add(vertexColor);

            TextBlock vertexLabel = new TextBlock
            {
                Text = "S( " + vertex.X + " | " + vertex.Y + " )",
                FontWeight = FontWeights.Bold
            };
            cont.Children.Add(vertexLabel);

            VertexPanel.Children.Add(cont);
        }

        private void RenderGraph(int index)
        {
            List<Point> points = CalcGraphPoints(index);
            Color clr = Formulas[index].Color;

            SolidColorBrush graphBrush = new SolidColorBrush(clr);

            Polygon graph = new Polygon
            {
                Stroke = graphBrush,
                StrokeThickness = 2,
                Points = new PointCollection(points)
            };

            mainCanvas.Children.Add(graph);
        }

        private void RenderAllGraphs()
        {
            VertexPanel.Children.Clear(); // Clear all vertex labels

            for (int i = 0; i < Formulas.Count; i++)
            {
                RenderGraph(i);
            }
        }

        private TextBlock CreateLineNumber(string text, double x, double y)
        {
            TextBlock lineNumber = new TextBlock
            {
                Text = text,
                FontSize = 16
            };
            Canvas.SetLeft(lineNumber, x);
            Canvas.SetTop(lineNumber, y);

            return lineNumber;
        }

        private void RenderLineNumbers()
        {
            double canvasMiddleX = CanvasWidth / 2;
            double canvasMiddleY = CanvasHeight / 2;

            int gridCountX = (int)(CanvasWidth / Scale);
            int gridCountY = (int)(CanvasHeight / Scale);

            // Horizontal line numbers
            for (int x = 1; x < gridCountX; x++)
            {
                TextBlock lineNumberPos = CreateLineNumber(x.ToString(), canvasMiddleX + Scale * x, canvasMiddleY);
                TextBlock lineNumberNeg = CreateLineNumber("-" + x, canvasMiddleX - Scale * x, canvasMiddleY);
                mainCanvas.Children.Add(lineNumberPos);
                mainCanvas.Children.Add(lineNumberNeg);
            }

            // Vertical line numbers
            for (int y = 1; y < gridCountY; y++)
            {
                TextBlock lineNumberPos = CreateLineNumber(y.ToString(), canvasMiddleX, canvasMiddleY - Scale * y);
                TextBlock lineNumberNeg = CreateLineNumber("-" + y, canvasMiddleX, canvasMiddleY + Scale * y);
                mainCanvas.Children.Add(lineNumberPos);
                mainCanvas.Children.Add(lineNumberNeg);
            }
        }

        private void RenderGrid()
        {
            mainCanvas.Children.Clear();

            CanvasWidth = mainCanvas.ActualWidth;
            CanvasHeight = mainCanvas.ActualHeight;
            double canvasMiddleX = CanvasWidth / 2;
            double canvasMiddleY = CanvasHeight / 2;

            OffsetX = (int)canvasMiddleX;
            OffsetY = (int)canvasMiddleY;

            int gridCountX = (int)(CanvasWidth / (Scale / 5));
            int gridCountY = (int)(CanvasHeight / (Scale / 5));

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

                Line linePos = new Line
                {
                    X1 = canvasMiddleX + Scale / 5 * x,
                    X2 = canvasMiddleX + Scale / 5 * x,
                    Y1 = 0,
                    Y2 = CanvasHeight,
                    Stroke = colorBrush,
                    StrokeThickness = strokeThickenss,
                };

                Line lineNeg = new Line
                {
                    X1 = canvasMiddleX - Scale / 5 * x,
                    X2 = canvasMiddleX - Scale / 5 * x,
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

                Line linePos = new Line
                {
                    X1 = 0,
                    X2 = CanvasWidth,
                    Y1 = canvasMiddleY + Scale / 5 * y,
                    Y2 = canvasMiddleY + Scale / 5 * y,
                    Stroke = colorBrush,
                    StrokeThickness = strokeThickenss,
                };

                Line lineNeg = new Line
                {
                    X1 = 0,
                    X2 = CanvasWidth,
                    Y1 = canvasMiddleY - Scale / 5 * y,
                    Y2 = canvasMiddleY - Scale / 5 * y,
                    Stroke = colorBrush,
                    StrokeThickness = strokeThickenss,
                };

                mainCanvas.Children.Add(linePos);
                mainCanvas.Children.Add(lineNeg);
            }

            // Crosshair
            Line crosshairX = new Line
            {
                X1 = 0,
                X2 = CanvasWidth,
                Y1 = (int)(CanvasHeight / 2),
                Y2 = (int)(CanvasHeight / 2),
                Stroke = crosshairBrush,
            };

            mainCanvas.Children.Add(crosshairX);

            Line crosshairY = new Line
            {
                X1 = (int)(CanvasWidth / 2),
                X2 = (int)(CanvasWidth / 2),
                Y1 = 0,
                Y2 = CanvasHeight,
                Stroke = crosshairBrush,
            };

            mainCanvas.Children.Add(crosshairY);
        }

        private void RenderCanvas()
        {
            RenderGrid();
            RenderAllGraphs();
            RenderLineNumbers();
        }

        private void ChangeZoom(double newScale)
        {
            Scale = newScale;
            RenderCanvas();
        }

        public void WindowLoaded(object sender, RoutedEventArgs e)
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            if (helper.Handle != null)
            {
                HwndSource source = HwndSource.FromHwnd(helper.Handle);
                if (source.Handle != null)
                {
                    source.AddHook(HwndMessageHook);
                }
            }

            RenderCanvas();
        }

        private void CalcButtonClick(object sender, RoutedEventArgs e)
        {
            RenderCanvas();
        }

        private void NewGraphButtonClick(object sender, RoutedEventArgs e)
        {
            AddGraph();
        }

        private void FormulaInputChanged(object sender, RoutedEventArgs e)
        {
            RenderCanvas();
        }

        public void ZoomSliderChange(object sender, RoutedEventArgs e)
        {
            Slider target = (Slider)sender;
            ChangeZoom(target.Value);
        }
    }
}

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

        public MainWindow()
        {
            InitializeComponent();
        }

        public void WindowLoaded(object sender, RoutedEventArgs e)
        {
            RenderGrid();
        }

        private void CalcButtonClick(object sender, RoutedEventArgs e)
        {
            RenderGrid();
        }

        private void RenderGrid()
        {
            mainCanvas.Children.Clear();

            double canvasWidth = mainCanvas.ActualWidth;
            double canvasHeight = mainCanvas.ActualHeight;
            double canvasMiddleX = canvasWidth / 2;
            double canvasMiddleY = canvasHeight / 2;

            int gridCountX = (int)(canvasWidth / UnitSize);
            int gridCountY = (int)(canvasHeight / UnitSize);

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
                    Y2 = canvasHeight,
                    Stroke = colorBrush,
                    StrokeThickness = strokeThickenss,
                };

                Line lineNeg = new Line {
                    X1 = canvasMiddleX - UnitSize * x,
                    X2 = canvasMiddleX - UnitSize * x,
                    Y1 = 0,
                    Y2 = canvasHeight,
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
                    X2 = canvasWidth,
                    Y1 = canvasMiddleY + UnitSize * y,
                    Y2 = canvasMiddleY + UnitSize * y,
                    Stroke = colorBrush,
                    StrokeThickness = strokeThickenss,
                };
                
                Line lineNeg = new Line {
                    X1 = 0,
                    X2 = canvasWidth,
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
                X2 = canvasWidth,
                Y1 = (int)(canvasHeight / 2),
                Y2 = (int)(canvasHeight / 2),
                Stroke = crosshairBrush,
            };
            
            mainCanvas.Children.Add(crosshairX);

            Line crosshairY = new Line {
                X1 = (int)(canvasWidth / 2),
                X2 = (int)(canvasWidth / 2),
                Y1 = 0,
                Y2 = canvasHeight,
                Stroke = crosshairBrush,
            };
            
            mainCanvas.Children.Add(crosshairY);
        }
    }
}

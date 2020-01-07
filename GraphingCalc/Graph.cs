using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace GraphingCalc
{
    class Graph
    {
        public TextBox InputA;
        public TextBox InputB;
        public TextBox InputC;
        public Color Color;

        public Graph(TextBox a, TextBox b, TextBox c, Color clr)
        {
            InputA = a;
            InputB = b;
            InputC = c;
            Color = clr;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FigmaSharp
{
    public class LineControl : TransparentControl
    {
        System.Drawing.Color color;
        public System.Drawing.Color LineColor {
            get => color;
            set {
                color = value;
            }
        }

        int lineWidth;
        public int LineWidth {
            get => lineWidth;
            set {
                lineWidth = value;
            }
        }

        public int X1, Y1, X2, Y2;
        public LineControl (System.Drawing.Color color)
        {
            LineColor = color;
        }

        protected override void CustomDraw(Graphics g)
        {
            base.CustomDraw (g);
            var myPen = new Pen (LineColor, LineWidth);
            g.DrawLine (myPen, X1, Y1, X2, Y2);
        }
    }
}

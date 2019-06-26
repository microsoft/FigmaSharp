using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FigmaSharp.WinForms
{
    public class LineTransparentControl : TransparentControl
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

        public Point Point1 { get; set; }
        public Point Point2 { get; set; }

        public LineTransparentControl (System.Drawing.Color color)
        {
            LineColor = color;
        }

        protected override void CustomDraw(Graphics g)
        {
            base.CustomDraw (g);
            var myPen = new Pen (LineColor, LineWidth);
            g.DrawLine (myPen, Point1.X, Point1.Y, Point2.X, Point2.Y);
        }
    }
}

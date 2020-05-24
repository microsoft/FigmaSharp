using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FigmaSharp
{
    public static class Extensions
    {
        public static Brush ToColor(this Color color)
        {
            return new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)(color.A * 255), (byte)(color.R * 255), (byte)(color.G * 255), (byte)(color.B * 255)));
        }

        public static Color ToFigmaColor(this Brush color)
        {
            if (color is SolidColorBrush solidColor)
            {
                return new Color()
                {
                    A = (float)solidColor.Color.A,
                    R = (float)solidColor.Color.R,
                    G = (float)solidColor.Color.G,
                    B = (float)solidColor.Color.B
                };
            }
            return new Color();
        }

        public static Color ToFigmaColor(this Color color)
        {
            return new Color() { A = (float)color.A, R = (float)color.R, G = (float)color.G, B = (float)color.B };
        }
    }
}

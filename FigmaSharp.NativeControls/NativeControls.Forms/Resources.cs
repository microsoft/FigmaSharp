using FigmaSharp;
using FigmaSharp.NativeControls;

namespace FigmaSharp.NativeControls
{
    public static class Resources
    {
        static CustomViewConverter[] converters;

        public static CustomViewConverter[] GetConverters()
        {
            if (converters == null)
            {
                converters = new CustomViewConverter[]
                {
                    new ButtonConverter ()
                };
            }

            return converters;
        }
    }
}
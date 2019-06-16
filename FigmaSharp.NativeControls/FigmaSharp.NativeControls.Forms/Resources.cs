using FigmaSharp;

namespace FigmaSharp.NativeControls.Forms
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
                    new ButtonConverter (),
                    new TextFieldConverter ()
                };
            }
            return converters;
        }
    }
}
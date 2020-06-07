using FigmaSharp.Controls.Forms.Converters;
using FigmaSharp.Converters;

namespace FigmaSharp.Controls.Forms
{
    public static class Resources
    {
        static NodeConverter[] converters;

        public static NodeConverter[] GetConverters()
        {
            if (converters == null)
            {
                converters = new NodeConverter[]
                {
                    new ButtonConverter (),
                    new TextFieldConverter ()
                };
            }
            return converters;
        }
    }
}
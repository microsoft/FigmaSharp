using FigmaSharp;
using FigmaSharp.Converters;
using FigmaSharp.PropertyConfigure;

namespace LocalFile.Shared
{
    public class FigmaStoryboard : FigmaFile
	{
        public FigmaStoryboard(NodeConverter[] figmaViewConverters = null, ViewPropertyConfigureBase propertySetter = null) : base ("FigmaStoryboard.figma", figmaViewConverters, propertySetter)
        {
            InitializeComponent();
        }
	}
}

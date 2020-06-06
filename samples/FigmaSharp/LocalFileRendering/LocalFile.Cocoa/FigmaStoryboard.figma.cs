using System;
using FigmaSharp;

namespace LocalFile.Shared
{
    public class FigmaStoryboard : FigmaFile
	{
        public FigmaStoryboard(LayerConverter[] figmaViewConverters = null, ViewPropertyNodeConfigureBase propertySetter = null) : base ("FigmaStoryboard.figma", figmaViewConverters, propertySetter)
        {
            InitializeComponent();
        }
	}
}

using System;
using FigmaSharp;

namespace LocalFile.Shared
{
    public class FigmaStoryboard : FigmaFile
	{
        public FigmaStoryboard(FigmaViewConverter[] figmaViewConverters) : base("FigmaStoryboard.figma", figmaViewConverters)
        {
            InitializeComponent();
        }
	}
}

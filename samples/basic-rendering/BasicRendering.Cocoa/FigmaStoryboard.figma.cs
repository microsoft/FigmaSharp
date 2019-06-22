using System;
using FigmaSharp;

namespace LocalFile.Cocoa
{
    public class FigmaStoryboard : FigmaFile
	{
		public FigmaStoryboard () : this (FigmaSharp.AppContext.Current.GetFigmaConverters())
		{
			InitializeComponent ();
		}

        public FigmaStoryboard (FigmaViewConverter[] figmaViewConverters) : base ("FigmaStoryboard.figma", figmaViewConverters)
        {

        }
	}
}

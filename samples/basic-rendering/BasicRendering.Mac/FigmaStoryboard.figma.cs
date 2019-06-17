using System;
using FigmaSharp;

namespace ExampleFigmaMac
{
    public class FigmaStoryboard : FigmaFile
	{
		public FigmaStoryboard () : this (FigmaSharp.AppContext.Current.GetFigmaConverters())
		{
			Initialize ();
		}

        public FigmaStoryboard (FigmaViewConverter[] figmaViewConverters) : base ("FigmaStoryboard.figma", figmaViewConverters)
        {

        }
	}
}

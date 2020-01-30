using System;
using System.Collections.Generic;
using System.Reflection;
using FigmaSharp.Views;
using FigmaSharp.Models;

namespace FigmaSharp
{
    public interface IFigmaDelegate
    {
        bool IsVerticalAxisFlipped { get; }

        IView CreateEmptyView();
        FigmaViewConverter[] GetFigmaConverters();

        IImage GetImage(string url);
		string GetSvgData(string url);

		IImage GetImageFromFilePath(string filePath);
        //FigmaResponse GetFigmaResponseFromContent(string template);
        string GetManifestResource(Assembly assembly, string file);

        IImage GetImageFromManifest(Assembly assembly, string imageRef);
        IImageView GetImageView(IImage image);
        void BeginInvoke(Action handler);
        FigmaCodePositionConverterBase GetPositionConverter();
        FigmaCodeAddChildConverterBase GetAddChildConverter();
    }
}

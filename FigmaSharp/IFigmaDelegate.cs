using System;
using System.Collections.Generic;
using System.Reflection;

namespace FigmaSharp
{
    public interface IFigmaDelegate
    {
        bool IsVerticalAxisFlipped { get; }

        IViewWrapper CreateEmptyView();
        FigmaViewConverter[] GetFigmaConverters();
        IImageWrapper GetImage(string url);
        IImageWrapper GetImageFromFilePath(string filePath);
        string GetFigmaFileContent(string file, string token);
        FigmaResponse GetFigmaResponseFromContent(string template);
        string GetManifestResource(Assembly assembly, string file);

        IImageWrapper GetImageFromManifest(Assembly assembly, string imageRef);
        IImageViewWrapper GetImageView(IImageWrapper image);
        void BeginInvoke(Action handler);
        FigmaCodePositionConverterBase GetPositionConverter();
        FigmaCodeAddChildConverterBase GetAddChildConverter();
    }
}

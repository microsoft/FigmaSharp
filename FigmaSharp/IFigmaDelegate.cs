using System.Collections.Generic;
using System.Reflection;

namespace FigmaSharp
{
    public interface IFigmaDelegate
    {
        IViewWrapper CreateEmptyView();
        FigmaViewConverter[] GetFigmaConverters();
        IImageWrapper GetImage(string url);
        IImageWrapper GetImageFromFilePath(string filePath);
        IImageViewWrapper GetImageView(FigmaPaint figmaPaint);
        string GetFigmaFileContent(string file, string token);
        FigmaResponse GetFigmaResponseFromContent(string template);
        string GetManifestResource(Assembly assembly, string file);

        IImageWrapper GetImageFromManifest(Assembly assembly, string imageRef);
        void LoadFigmaFromFrameEntity(IViewWrapper contentView, FigmaResponse figmaResponse, List<IImageViewWrapper> figmaImages, int page);
    }
}

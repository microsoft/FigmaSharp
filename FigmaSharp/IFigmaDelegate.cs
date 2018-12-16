using System.Reflection;

namespace FigmaSharp
{
    public interface IFigmaDelegate
    {
        FigmaViewConverter[] GetFigmaConverters();
        IImageWrapper GetImage(string url);
        IImageWrapper GetImageFromFilePath(string filePath);
        IImageViewWrapper GetImageView(FigmaPaint figmaPaint);
        IImageWrapper GetImageFromManifest(Assembly assembly, string imageRef);
    }
}

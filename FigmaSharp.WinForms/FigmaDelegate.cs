using System.Reflection;

namespace FigmaSharp
{
    public class FigmaDelegate : IFigmaDelegate
    {
        public FigmaDelegate()
        {
        }

        public FigmaViewConverter[] GetFigmaConverters()
        {
            throw new System.NotImplementedException();
        }

        public IImageWrapper GetImage(string url)
        {
            throw new System.NotImplementedException();
        }

        public IImageWrapper GetImageFromFilePath(string filePath)
        {
            throw new System.NotImplementedException();
        }

        public IImageWrapper GetImageFromManifest(Assembly assembly, string imageRef)
        {
            throw new System.NotImplementedException();
        }

        public IImageViewWrapper GetImageView(FigmaPaint figmaPaint)
        {
            throw new System.NotImplementedException();
        }
    }
}

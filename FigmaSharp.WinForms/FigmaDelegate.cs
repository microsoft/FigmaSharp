using System.Collections.Generic;
using System.Reflection;

namespace FigmaSharp
{
    public class FigmaDelegate : IFigmaDelegate
    {
        public FigmaDelegate()
        {
        }

        public IViewWrapper CreateEmptyView()
        {
            throw new System.NotImplementedException();
        }

        public FigmaViewConverter[] GetFigmaConverters()
        {
            throw new System.NotImplementedException();
        }

        public IFigmaDocumentContainer GetFigmaDialogFromContent(string template)
        {
            throw new System.NotImplementedException();
        }

        public string GetFigmaFileContent(string file, string token)
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

        public string GetManifestResource(Assembly assembly, string file)
        {
            throw new System.NotImplementedException();
        }

        public void LoadFigmaFromFrameEntity(IViewWrapper contentView, IFigmaDocumentContainer document, List<IImageViewWrapper> figmaImages, string figmaFileName)
        {
            throw new System.NotImplementedException();
        }
    }
}

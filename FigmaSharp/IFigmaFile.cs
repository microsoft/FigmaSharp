using System.Collections.Generic;

namespace FigmaSharp
{
    public interface IFigmaFile
    {
        List<IImageViewWrapper> FigmaImages { get; }
        IFigmaDocumentContainer Document { get; }

        void Initialize();

        void Reload(bool includeImages = false);

        void ReloadImages();
    }
}

using System.Collections.Generic;

namespace FigmaSharp.WinForms
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

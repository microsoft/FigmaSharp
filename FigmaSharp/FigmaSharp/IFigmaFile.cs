using System.Collections.Generic;
using FigmaSharp.Models;
using FigmaSharp.Views;

namespace FigmaSharp
{
    public interface IFigmaFile
    {
        List<IImageView> FigmaImages { get; }
        FigmaFileResponse Document { get; }

        void InitializeComponent();

        void Reload ();
    }
}

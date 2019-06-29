using System.Collections.Generic;
using FigmaSharp.Models;

namespace FigmaSharp
{
    public interface IFigmaFile
    {
        List<IImageViewWrapper> FigmaImages { get; }
        FigmaResponse Document { get; }

        void InitializeComponent();

        void Reload ();
    }
}

using System.Collections.Generic;
using FigmaSharp.Models;
using LiteForms;

namespace FigmaSharp
{
    public interface IFigmaFile
    {
        List<IImageView> FigmaImages { get; }
        FigmaResponse Document { get; }

        void InitializeComponent();

        void Reload ();
    }
}

using System.Collections.Generic;
using FigmaSharp;

namespace FigmaSharp.Services
{
    public interface IRendererService<T> where T : IViewWrapper
    {
        ProcessedNode[] MainViews { get; }
        void Start(T targetView, FigmaFileService figmaFileService);
    }
}

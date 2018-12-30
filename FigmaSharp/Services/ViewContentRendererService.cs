using System;
using System.Collections.Generic;

namespace FigmaSharp.Services
{
    public class ViewContentRendererService : IRendererService<IViewWrapper>
    {
        public ProcessedNode[] MainViews => throw new System.NotImplementedException();

        public void Start(IViewWrapper targetView, FigmaFileService figmaFileService)
        {

        }
    }
}

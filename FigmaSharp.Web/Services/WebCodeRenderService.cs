using System;
using FigmaSharp.Converters;
using FigmaSharp.PropertyConfigure;
using FigmaSharp.Services;

namespace FigmaSharp.Web.Services
{
    public class WebCodeRenderService : CodeRenderService
    {
        public WebCodeRenderService(INodeProvider figmaProvider, NodeConverter[] figmaViewConverters, CodePropertyConfigureBase codePropertyConverter, ICodeNameService codeNameService = null) : base(figmaProvider, figmaViewConverters, codePropertyConverter, codeNameService)
        {
        }
    }
}

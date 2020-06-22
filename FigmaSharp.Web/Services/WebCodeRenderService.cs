using System;
using System.Collections.Generic;
using FigmaSharp.Converters;
using FigmaSharp.PropertyConfigure;
using FigmaSharp.Services;

namespace FigmaSharp.Web.Services
{

    public class WebCodeRenderService : RenderService
	{
		readonly internal List<CodeNode> Nodes = new List<CodeNode>();

		internal CodeRenderServiceOptions CurrentRendererOptions { get; set; }

		public WebCodeRenderService(INodeProvider figmaProvider, NodeConverter[] figmaViewConverters,
			CodePropertyConfigureBase codePropertyConverter, ICodeNameService codeNameService = null) : base(figmaProvider, figmaViewConverters)
		{
		
		}
	}
}

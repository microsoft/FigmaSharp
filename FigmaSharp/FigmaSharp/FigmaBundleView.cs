using System;
using System.IO;
using FigmaSharp.Services;

namespace FigmaSharp
{
	public class FigmaBundleView
	{
		internal const string PublicCsExtension = ".cs";
		internal const string PartialDesignerExtension = ".designer.cs";

		public Models.FigmaNode FigmaNode { get; }
		public string Name { get; private set; }

		public string PartialDesignerClassName => $"{Name}{PartialDesignerExtension}";
		public string PartialDesignerClassFilePath =>
			Path.Combine (bundle.ViewsDirectoryPath, PartialDesignerClassName);

		public string PublicCsClassName => $"{Name}{PublicCsExtension}";
		public string PublicCsClassFilePath =>
			Path.Combine (bundle.ViewsDirectoryPath, PublicCsClassName);

		public string RemoteFileUrl => $"https://www.figma.com/file/{bundle.FileId}/";

		private readonly FigmaBundle bundle;

		public FigmaBundleItemBase (FigmaBundle figmaBundle, string viewName, Models.FigmaNode figmaNode)
		{
			Name = viewName;
			bundle = figmaBundle;
			FigmaNode = figmaNode;
		}

		public FigmaPartialDesignerClass GetFigmaPartialDesignerClass (FigmaCodeRendererService codeRendererService)
		{
			var partialDesignerClass = new FigmaPartialDesignerClass ();
			partialDesignerClass.Manifest = new FigmaManifest () {
				Date = DateTime.Now,
				FileId = bundle.FileId,
				DocumentVersion = 0.1f,
				RemoteApiVersion = AppContext.Api.Version.ToString (),
				ApiVersion = AppContext.Current.Version
			};

			string initializeComponentContent;
			if (FigmaNode != null) {
				var builder = new System.Text.StringBuilder ();

				var isEnabled = codeRendererService.MainIsThis;
				codeRendererService.MainIsThis = true;
				codeRendererService.GetCode (builder, FigmaNode, null, null);
				initializeComponentContent = builder.ToString ();

				codeRendererService.MainIsThis = isEnabled;

			} else {
				initializeComponentContent = string.Empty;
			}

			partialDesignerClass.Usings.Add ("AppKit");
			partialDesignerClass.ClassName = Name;
			partialDesignerClass.Namespace = bundle.Namespace;
			partialDesignerClass.InitializeComponentContent = initializeComponentContent;

			return partialDesignerClass;
		}

		public FigmaPublicPartialClass GetPublicPartialClass ()
		{
			var publicPartialClass = new FigmaPublicPartialClass ();
			publicPartialClass.Usings.Add ("AppKit");
			publicPartialClass.ClassName = Name;
			publicPartialClass.Namespace = bundle.Namespace;
			publicPartialClass.BaseClass = "AppKit.NSView";
			return publicPartialClass;
		}

		public void Generate (FigmaCodeRendererService codeRendererService)
		{
			if (!Directory.Exists (bundle.ViewsDirectoryPath))
				Directory.CreateDirectory (bundle.ViewsDirectoryPath);

			var partialDesignerClass = GetFigmaPartialDesignerClass (codeRendererService);
			partialDesignerClass.Save (PartialDesignerClassFilePath);

			var publicPartialClass = GetPublicPartialClass ();
			publicPartialClass.Save (PublicCsClassFilePath);
		}
	}
}

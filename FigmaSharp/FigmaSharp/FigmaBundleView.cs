using System;
using System.IO;
using FigmaSharp.Services;

namespace FigmaSharp
{
	public class FigmaBundleView
	{
		internal const string PublicCsExtension = ".cs";
		internal const string PartialDesignerExtension = ".designer.cs";

		public Models.FigmaNode FigmaNodeName { get; private set; }
		public string Name { get; private set; }

		public string PartialDesignerClassName => $"{Name}{PartialDesignerExtension}";
		public string PartialDesignerClassFilePath =>
			Path.Combine (bundle.ViewsDirectoryPath, PartialDesignerClassName);

		public string PublicCsClassName => $"{Name}{PublicCsExtension}";
		public string PublicCsClassFilePath =>
			Path.Combine (bundle.ViewsDirectoryPath, PublicCsClassName);

		private readonly FigmaBundle bundle;

		public FigmaBundleView (FigmaBundle figmaBundle, string viewName, Models.FigmaNode figmaName)
		{
			Name = viewName;
			bundle = figmaBundle;
			FigmaNodeName = figmaName;
		}

		public FigmaPartialDesignerClass GetFigmaPartialDesignerClass (IFigmaFileProvider fileProvider, FigmaCodeRendererService codeRendererService)
		{
			var partialDesignerClass = new FigmaPartialDesignerClass ();
			partialDesignerClass.Manifest = new FigmaManifest () {
				Date = DateTime.Now,
				DocumentUrl = "https://www.figma.com/file/fKugSkFGdwOF4vDsPGnJee/",
				DocumentVersion = 0.1f,
				RemoteApiVersion = AppContext.Api.Version.ToString (),
				ApiVersion = AppContext.Current.Version
			};

			string initializeComponentContent;
			if (FigmaNodeName != null) {
				var builder = new System.Text.StringBuilder ();
				codeRendererService.GetCode (builder, FigmaNodeName, null, null);

				initializeComponentContent = builder.ToString ();
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

		public void Generate (IFigmaFileProvider fileProvider, FigmaCodeRendererService codeRendererService)
		{
			if (!Directory.Exists (bundle.ViewsDirectoryPath))
				Directory.CreateDirectory (bundle.ViewsDirectoryPath);

			var partialDesignerClass = GetFigmaPartialDesignerClass (fileProvider, codeRendererService);
			partialDesignerClass.Save (PartialDesignerClassFilePath);

			var publicPartialClass = GetPublicPartialClass ();
			publicPartialClass.Save (PublicCsClassFilePath);
		}
	}
}

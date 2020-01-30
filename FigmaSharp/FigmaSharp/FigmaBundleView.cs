using System;
using System.IO;
using FigmaSharp.Services;

namespace FigmaSharp
{
	public class FigmaBundleView
	{
		internal const string PublicCsExtension = ".cs";
		internal const string PartialDesignerExtension = ".designer.cs";

		public string FigmaNodeName { get; private set; }
		public string Name { get; private set; }

		public string PartialDesignerClassName => $"{Name}{PartialDesignerExtension}";
		public string PartialDesignerClassFilePath =>
			Path.Combine (bundle.ViewsDirectoryPath, PartialDesignerClassName);

		public string PublicCsClassName => $"{Name}{PublicCsExtension}";
		public string PublicCsClassFilePath =>
			Path.Combine (bundle.ViewsDirectoryPath, PublicCsClassName);

		private readonly FigmaBundle bundle;

		public FigmaBundleView (FigmaBundle figmaBundle, string viewName, string figmaName)
		{
			Name = viewName;
			bundle = figmaBundle;
			FigmaNodeName = figmaName;
		}

		public void Generate (IFigmaFileProvider fileProvider, FigmaCodeRendererService codeRendererService)
		{
			if (!Directory.Exists (bundle.ViewsDirectoryPath))
				Directory.CreateDirectory (bundle.ViewsDirectoryPath);

			var partialDesignerClass = new FigmaPartialDesignerClass ();
			partialDesignerClass.Manifest = new FigmaManifest () {
				Date = DateTime.Now,
				DocumentUrl = "https://www.figma.com/file/fKugSkFGdwOF4vDsPGnJee/",
				DocumentVersion = 0.1f,
				RemoteApiVersion = AppContext.Api.Version,
				ApiVersion = AppContext.Current.Version
			};

			string initializeComponentContent;
			if (!string.IsNullOrEmpty (FigmaNodeName)) {
				var figmaNode = fileProvider.FindByName (FigmaNodeName);
				if (figmaNode == null) {
					throw new Exception ("node not found");
				}
				var builder = new System.Text.StringBuilder ();
				codeRendererService.GetCode (builder, figmaNode, null, null);

				initializeComponentContent = builder.ToString ();
			} else {
				initializeComponentContent = string.Empty;
			}

			partialDesignerClass.Usings.Add ("AppKit");
			partialDesignerClass.ClassName = Name;
			partialDesignerClass.Namespace = bundle.Namespace;
			partialDesignerClass.InitializeComponentContent = initializeComponentContent;

			partialDesignerClass.Save (PartialDesignerClassFilePath);

			var partialDesignerClassCode = partialDesignerClass.Generate ();
			File.WriteAllText (PartialDesignerClassFilePath, partialDesignerClassCode);

			var publicPartialClass = new FigmaPublicPartialClass ();
			publicPartialClass.Usings.Add ("AppKit");
			publicPartialClass.ClassName = Name;
			publicPartialClass.Namespace = bundle.Namespace;
			publicPartialClass.BaseClass = "AppKit.NSView";
			publicPartialClass.Save (PublicCsClassFilePath);
		}
	}
}

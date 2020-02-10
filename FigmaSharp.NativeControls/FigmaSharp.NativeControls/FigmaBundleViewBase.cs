using System;
using System.IO;
using FigmaSharp.Services;

namespace FigmaSharp
{
	public abstract class FigmaBundleViewBase
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

		public FigmaBundleViewBase (FigmaBundle figmaBundle, string viewName, Models.FigmaNode figmaNode)
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
				DocumentVersion = "0.1f",
				RemoteApiVersion = AppContext.Api.Version.ToString (),
				ApiVersion = AppContext.Current.Version
			};

			partialDesignerClass.ClassName = Name;
			partialDesignerClass.Namespace = bundle.Namespace;

			OnGetPartialDesignerClass (partialDesignerClass, codeRendererService);

			return partialDesignerClass;
		}

		protected abstract void OnGetPartialDesignerClass (FigmaPartialDesignerClass partialDesignerClass, FigmaCodeRendererService codeRendererService);

		protected abstract void OnGetPublicDesignerClass (FigmaPublicPartialClass publicPartialClass);

		public FigmaPublicPartialClass GetPublicPartialClass ()
		{
			var publicPartialClass = new FigmaPublicPartialClass ();
		
			publicPartialClass.ClassName = Name;
			publicPartialClass.Namespace = bundle.Namespace;

			OnGetPublicDesignerClass (publicPartialClass);

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

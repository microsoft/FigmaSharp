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
		public FigmaBundle Bundle => bundle;

		public FigmaBundleViewBase (FigmaBundle figmaBundle, string viewName, Models.FigmaNode figmaNode)
		{
			Name = viewName;
			bundle = figmaBundle;
			FigmaNode = figmaNode;
		}

		public FigmaPartialDesignerClass GetFigmaPartialDesignerClass (FigmaCodeRendererService codeRendererService, string namesSpace = null, bool translateStrings = false)
		{
			var partialDesignerClass = new FigmaPartialDesignerClass ();
			partialDesignerClass.Manifest = new FigmaManifest () {
				Date = DateTime.Now,
				FileId = bundle.FileId,
				DocumentVersion = "0.1f",
				Namespace = namesSpace ?? FigmaBundle.DefaultNamespace,
				RemoteApiVersion = AppContext.Api.Version.ToString (),
				ApiVersion = AppContext.Current.Version
			};

			partialDesignerClass.ClassName = Name;
			partialDesignerClass.Namespace = bundle.Namespace;

			OnGetPartialDesignerClass (partialDesignerClass, codeRendererService, translateStrings);

			return partialDesignerClass;
		}

		protected abstract void OnGetPartialDesignerClass (FigmaPartialDesignerClass partialDesignerClass, FigmaCodeRendererService codeRendererService, bool translateStrings);

		protected abstract void OnGetPublicDesignerClass (FigmaPublicPartialClass publicPartialClass);

		public FigmaPublicPartialClass GetPublicPartialClass ()
		{
			var publicPartialClass = new FigmaPublicPartialClass {
				ClassName = Name,
				Namespace = bundle.Namespace
			};

			OnGetPublicDesignerClass (publicPartialClass);

			return publicPartialClass;
		}

		public void Generate (FigmaCodeRendererService codeRendererService, bool writePublicClassIfExists = true, string namesSpace = null, bool translateStrings = false)
		{
			Generate(bundle.ViewsDirectoryPath, codeRendererService, writePublicClassIfExists, namesSpace, translateStrings);
		}

		public void GeneratePublicPartialClass (string directoryPath, bool writePublicClassIfExists = true)
		{
			var publicCsClassFilePath = Path.Combine(directoryPath, PublicCsClassName);
			if (!File.Exists(publicCsClassFilePath) || writePublicClassIfExists)
			{
				var publicPartialClass = GetPublicPartialClass();
				publicPartialClass.Save (publicCsClassFilePath);
			}
		}

		public void GeneratePartialDesignerClass(FigmaCodeRendererService codeRendererService, string directoryPath, string namesSpace = null, bool translateStrings = false)
		{
			var partialDesignerClass = GetFigmaPartialDesignerClass(codeRendererService, namesSpace, translateStrings);

			var partialDesignerClassFilePath = Path.Combine(directoryPath, PartialDesignerClassName);
			partialDesignerClass.Save(partialDesignerClassFilePath);
		}

			public void Generate (string directoryPath, FigmaCodeRendererService codeRendererService, bool writePublicClassIfExists = true, string namesSpace = null, bool translateStrings = false)
		{
			if (!Directory.Exists(directoryPath))
				Directory.CreateDirectory(directoryPath);

			GeneratePartialDesignerClass(codeRendererService, directoryPath, namesSpace, translateStrings);
			GeneratePublicPartialClass(directoryPath, writePublicClassIfExists);
		}
	}
}

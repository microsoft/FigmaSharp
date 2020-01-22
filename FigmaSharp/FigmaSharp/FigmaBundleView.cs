using System;
using System.IO;

namespace FigmaSharp
{
	public class FigmaBundleView
	{
		internal const string PublicCsExtension = ".cs";
		internal const string PartialDesignerExtension = ".designer.cs";

		public string Name { get; private set; }

		public string PartialDesignerClassName => $"{Name}{PartialDesignerExtension}";
		public string PartialDesignerClassFilePath =>
			Path.Combine (bundle.ViewsDirectoryPath, PartialDesignerClassName);

		public string PublicCsClassName => $"{Name}{PublicCsExtension}";
		public string PublicCsClassFilePath =>
			Path.Combine (bundle.ViewsDirectoryPath, PublicCsClassName);

		private readonly FigmaBundle bundle;

		public FigmaBundleView (FigmaBundle figmaBundle, string name)
		{
			Name = name;
			bundle = figmaBundle;
		}

		public void Generate ()
		{
			if (!Directory.Exists (bundle.ViewsDirectoryPath))
				Directory.CreateDirectory (bundle.ViewsDirectoryPath);

			var partialDesignerClass = new FigmaPartialDesignerClass ();
			partialDesignerClass.Manifest = new FigmaManifest () {
				Date = DateTime.Now,
				DocumentUrl = "https://www.figma.com/file/fKugSkFGdwOF4vDsPGnJee/",
				DocumentVersion = 0.1f,
				RemoteApiVersion = "1.1"
			};

			partialDesignerClass.Usings.Add ("AppKit");
			partialDesignerClass.ClassName = Name;
			partialDesignerClass.Namespace = bundle.Namespace;

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

// Authors:
//   Jose Medrano <josmed@microsoft.com>
//
// Copyright (C) 2018 Microsoft, Corp
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to permit
// persons to whom the Software is furnished to do so, subject to the
// following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
// NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
// USE OR OTHER DEALINGS IN THE SOFTWARE.

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

		public FigmaPartialDesignerClass GetFigmaPartialDesignerClass (CodeRenderService codeRendererService, string namesSpace = null, bool translateStrings = false)
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

		protected abstract void OnGetPartialDesignerClass (FigmaPartialDesignerClass partialDesignerClass, CodeRenderService codeRendererService, bool translateStrings);

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

		public void Generate (CodeRenderService codeRendererService, bool writePublicClassIfExists = true, string namesSpace = null, bool translateStrings = false)
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

		public void GeneratePartialDesignerClass(CodeRenderService codeRendererService, string directoryPath, string namesSpace = null, bool translateStrings = false)
		{
			var partialDesignerClass = GetFigmaPartialDesignerClass(codeRendererService, namesSpace, translateStrings);

			var partialDesignerClassFilePath = Path.Combine(directoryPath, PartialDesignerClassName);
			partialDesignerClass.Save(partialDesignerClassFilePath);
		}

			public void Generate (string directoryPath, CodeRenderService codeRendererService, bool writePublicClassIfExists = true, string namesSpace = null, bool translateStrings = false)
		{
			if (!Directory.Exists(directoryPath))
				Directory.CreateDirectory(directoryPath);

			GeneratePartialDesignerClass(codeRendererService, directoryPath, namesSpace, translateStrings);
			GeneratePublicPartialClass(directoryPath, writePublicClassIfExists);
		}
	}
}

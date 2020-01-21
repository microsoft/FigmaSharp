using System;
using System.Collections.Generic;
using System.Text;

namespace FigmaSharp
{
	public class FigmaPartialDesignerClass : FigmaClassBase
	{
		public string Namespace { get; set; }
		public string ClassName { get; set; }

		public string InitializeComponentContent { get; set; }

		public FigmaPartialDesignerClass ()
		{
		}

		protected void GeneratePartialDesignerClass (StringBuilder sb, string className)
		{
			AppendLine (sb, $"partial class {className}");
			OpenBracket (sb);
		}
		protected void ClosePartialDesignerClass (StringBuilder sb) => CloseBracket (sb);

		protected void GenerateInitializeComponentMethod (StringBuilder sb)
		{
			GeneratePrivateMethod (sb, "InitializeComponent");
			sb.AppendLine (InitializeComponentContent);
			RemoveTabLevel ();
			CloseBracket (sb);
		}

		protected void CloseInitializeComponentMethod (StringBuilder sb) => CloseBracket (sb);

		protected override string OnGenerate ()
		{
			var sb = new StringBuilder ();
			GenerateComments (sb);
			GenerateUsings (sb);
			GenerateNamespace (sb, Namespace);
			GeneratePartialDesignerClass (sb, ClassName);
			GenerateInitializeComponentMethod (sb);
			ClosePartialDesignerClass (sb);
			CloseNamespace (sb);
			return sb.ToString ();
		}
	}

	public class FigmaPublicPartialClass : FigmaClassBase
	{
		public FigmaPublicPartialClass ()
		{
		}

		public string Namespace { get; set; }
		public string ClassName { get; set; }
		public string BaseClass { get; set; }

		protected void GeneratePublicPartialClass (StringBuilder sb, string className, string baseClassName)
		{
			baseClassName = !string.IsNullOrEmpty (baseClassName) ? $" : {baseClassName}" : string.Empty;
			AppendLine (sb, $"public partial class {className}{baseClassName}");
			OpenBracket (sb);
		}

		protected void ClosePublicPartialClass (StringBuilder sb) => CloseBracket (sb);

		protected override string OnGenerate ()
		{
			var sb = new StringBuilder ();
			GenerateComments (sb);
			GenerateUsings (sb);
			GenerateNamespace (sb, Namespace);
			GeneratePublicPartialClass (sb, ClassName, BaseClass);
			GenerateConstructor (sb, ClassName);
			RemoveTabLevel ();
			CloseConstructor (sb);
			ClosePublicPartialClass (sb);
			CloseNamespace (sb);
			return sb.ToString ();
		}
	}

	public abstract class FigmaClassBase
	{
		public List<string> Usings { get; } = new List<string> ();
		public List<string> Comments { get; } = new List<string> ();

		public FigmaManifest Manifest { get; set; }

		public bool ShowManifestComments => Manifest != null;

		int CurrentTabIndex = 0;

		protected void RemoveTabLevel () => CurrentTabIndex--;

		protected void GenerateComments (StringBuilder builder)
		{
			if (ShowManifestComments) {
				Manifest.ToComment (builder);
				builder.AppendLine ("* ");
			}
			foreach (var current in Comments) {
				builder.AppendLine ($"* {current}");
			}
		}

		public void Save (string filePath)
		{
			var code = Generate ();
			try {
				if (System.IO.File.Exists (filePath))
					System.IO.File.Delete (filePath);
				System.IO.File.WriteAllText (filePath, code);
			} catch (Exception ex) {
				System.Diagnostics.Debug.Fail (ex.ToString ());
			}
		}

		protected void GenerateUsings (StringBuilder builder)
		{
			foreach (var current in Usings) {
				builder.AppendLine ($"using {current};");
			}
		}

		protected void GeneratePrivateMethod (StringBuilder sb, string methodName)
		{
			AppendLine (sb, $"private void {methodName} ()");
			OpenBracket (sb);
		}
		protected void ClosePrivateMethod (StringBuilder sb) => CloseBracket (sb);

		protected void GenerateNamespace (StringBuilder sb, string fullNamespace)
		{
			AppendLine(sb, $"namespace {fullNamespace}");
			OpenBracket (sb);
		}
		protected void CloseNamespace (StringBuilder sb) => CloseBracket (sb);

		protected void GenerateConstructor (StringBuilder sb, string className)
		{
			AppendLine (sb, $"public {className} ()");
			OpenBracket (sb);
		}
		protected void CloseConstructor (StringBuilder sb) => CloseBracket (sb);

		protected void CloseBracket (StringBuilder sb)
		{
			AppendLine (sb, "}");
			CurrentTabIndex--;
		}

		protected void OpenBracket (StringBuilder sb)
		{
			AppendLine (sb, "{");
			CurrentTabIndex++;
		}

		protected void AppendLine (StringBuilder sb, string line) =>
			sb.AppendLine ($"{new string ('\t', CurrentTabIndex)}{line}");

		public string Generate ()
		{
			CurrentTabIndex = 0;
			return OnGenerate ();
		}

		protected abstract string OnGenerate ();
	}
}

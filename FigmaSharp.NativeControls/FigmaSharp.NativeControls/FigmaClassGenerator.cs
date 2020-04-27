using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace FigmaSharp
{
	public enum MethodModifier
	{
		Private,
		Public,
		Protected
	}

	public class ClassMethod
	{
		public MethodModifier MethodModifier { get; set; } = MethodModifier.Private;

		public List<(string, string)> Args = new List<(string, string)>();

		public string MethodName { get; protected set; }

		public ClassMethod()
		{
		
		}

		public virtual void Write(FigmaClassBase figmaClassBase, StringBuilder sb)
		{
			figmaClassBase.GenerateMethod(sb, MethodName, MethodModifier.Public);
			Write (figmaClassBase, sb);
			figmaClassBase.CloseBracket(sb);
		}
	}

	public class FigmaPartialDesignerClass : FigmaClassBase
	{
		public List<(Type memberType, string name)> PrivateMembers = new List<(Type memberType, string name)>();
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
			GenerateMethod (sb, InitializeComponentMethodName);

			if (InitializeComponentContent != null) {
				foreach (var line in InitializeComponentContent.Split ('\n')) {
					AppendLine (sb, line);
				}
			}
			RemoveTabLevel ();
			CloseMethod (sb);
		}


		protected virtual void GenerateMethods (StringBuilder sb)
		{
			foreach (var method in this.Methods)
			{

				sb.AppendLine();
				method.Write(this, sb);
            }
		}

		protected void GenerateMembers(StringBuilder sb)
		{
			//private members
			var groupedMembers = PrivateMembers
				.Select(s => s.memberType)
				.Distinct ();

			foreach (var member in groupedMembers)
			{
				var items = PrivateMembers
					.Where(s => s.memberType == member)
					.Select (s => s.name)
					.ToArray ();

				var separatedValues = string.Join(", ", items);
				AppendLine(sb, $"private {member.FullName} {separatedValues};");
			}
			AppendLine (sb);
		}

		protected void CloseInitializeComponentMethod (StringBuilder sb) => CloseBracket (sb);

		protected override string OnGenerate ()
		{
			var sb = new StringBuilder ();
			GenerateComments (sb);
			GenerateUsings (sb);
			GenerateNamespace (sb, Namespace);
			GeneratePartialDesignerClass (sb, ClassName);
			GenerateMembers (sb);
			GenerateInitializeComponentMethod (sb);
			GenerateMethods (sb);
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
			AppendLine (sb, $"{InitializeComponentMethodName} ();");
			RemoveTabLevel ();
			CloseConstructor (sb);
			ClosePublicPartialClass (sb);
			CloseNamespace (sb);
			return sb.ToString ();
		}
	}

	public abstract class FigmaClassBase
	{
		protected const string InitializeComponentMethodName = "InitializeComponent";

		public List<ClassMethod> Methods = new List<ClassMethod>();

		public List<string> Usings { get; } = new List<string>();
		public List<string> Comments { get; } = new List<string>();

		public FigmaManifest Manifest { get; set; }

		public bool ShowManifestComments => Manifest != null;

		protected int CurrentTabIndex = 0;

		public void RemoveTabLevel() => CurrentTabIndex--;
		public void AddTabLevel() => CurrentTabIndex++;

		protected void GenerateComments(StringBuilder builder)
		{
			if (ShowManifestComments) {
				Manifest.ToComment(builder);
			}
			foreach (var current in Comments) {
				builder.AppendLine($"// {current}");
			}
		}

		public void Save(string filePath)
		{
			var code = Generate();
			try {
				if (System.IO.File.Exists(filePath))
					System.IO.File.Delete(filePath);
				System.IO.File.WriteAllText(filePath, code);
			} catch (Exception ex) {
				System.Diagnostics.Debug.Fail(ex.ToString());
			}
		}

		protected void GenerateUsings(StringBuilder builder)
		{
			builder.AppendLine();

			foreach (var current in Usings)
				builder.AppendLine($"using {current};");
		}

		internal void GenerateMethod (StringBuilder sb, string methodName,
			MethodModifier modifier = MethodModifier.Private, List<(string, string)> arguments = null)
		{
			string args = string.Empty;

			if (arguments != null)
			{
				for (int i = 0; i < arguments.Count; i++)
				{
					var argument = arguments[i];

					if (i > 0)
						args += ", ";

					args += $"{argument.Item1} {argument.Item2}";
				}
			}

			AppendLine(sb, $"{modifier.ToString ().ToLower ()} void {methodName} ({args})");
			OpenBracket(sb);
		}

		internal void CloseMethod (StringBuilder sb) => CloseBracket (sb);

		protected void GenerateNamespace (StringBuilder sb, string fullNamespace)
		{
			sb.AppendLine();
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

		internal void CloseBracket (StringBuilder sb)
		{
			AppendLine (sb, "}");
			CurrentTabIndex--;
		}

		internal void OpenBracket (StringBuilder sb)
		{
			AppendLine (sb, "{");
			CurrentTabIndex++;
		}

		public void AppendLine(StringBuilder sb, string line) {
			if (string.IsNullOrWhiteSpace(line)) {
				sb.AppendLine();
				return;
			}
			sb.AppendLine($"{new string('\t', CurrentTabIndex)}{line}");
		}

		protected void AppendLine(StringBuilder sb) => sb.AppendLine();

		public string Generate ()
		{
			CurrentTabIndex = 0;
			return OnGenerate ();
		}

		protected abstract string OnGenerate ();
	}
}

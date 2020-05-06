using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace FigmaSharp
{
	public enum CodeObjectModifier
	{
		Private,
		Public,
		Protected
	}

    public abstract class CodeObject
    {
		public string Name { get; private set; }

		public CodeObject (string name)
        {
			Name = name;

		}

		public CodeObjectModifier MethodModifier { get; set; } = CodeObjectModifier.Private;

		abstract public void Write(FigmaClassBase figmaClassBase, StringBuilder sb);
	}

	public class EnumCodeObject : CodeObject
	{
		public List<Models.FigmaFrameEntity> Values;

		public EnumCodeObject(string name, List<Models.FigmaFrameEntity> values) : base(name)
		{
			Values = values;
		}

		public override void Write(FigmaClassBase figmaClassBase, StringBuilder sb)
		{
			figmaClassBase.AddTabLevel();
			figmaClassBase.GenerateEnum(sb, Name, CodeObjectModifier.Public);
			for (int i = 0; i < Values.Count; i++)
			{
				var comma = (i < Values.Count - 1) ? "," : "";
				figmaClassBase.AppendLine(sb, $"{Values[i].GetClassName ()}{comma}");
			}
			figmaClassBase.RemoveTabLevel();
			figmaClassBase.CloseBracket(sb);
		}
	}
	
    public class ClassMethodCodeObject : CodeObject
	{
		public List<(string, string)> Args = new List<(string, string)>();

		public ClassMethodCodeObject (string name) : base(name)
		{
		
		}

		public override void Write(FigmaClassBase figmaClassBase, StringBuilder sb)
		{
			figmaClassBase.GenerateMethod(sb, Name, CodeObjectModifier.Public);
			Write (figmaClassBase, sb);
			figmaClassBase.CloseBracket(sb);
		}
	}

	public class FigmaPartialDesignerClass : FigmaClassBase
	{
		public List<(string memberType, string name)> PrivateMembers = new List<(string memberType, string name)>();
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
				AppendLine(sb);
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
				AppendLine(sb, $"private {member} {separatedValues};");
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

		public List<CodeObject> Methods = new List<CodeObject>();

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

		internal void GenerateEnum(StringBuilder sb, string name, CodeObjectModifier objectModifier)
		{
			AppendLine(sb, $"{objectModifier.ToString().ToLower()} enum {name}");
			OpenBracket(sb);
		}

		internal void GenerateMethod (StringBuilder sb, string methodName,
			CodeObjectModifier modifier = CodeObjectModifier.Private, List<(string, string)> arguments = null)
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

		internal void CloseBracket (StringBuilder sb, bool removeTabIndex = true)
		{
			AppendLine (sb, "}");
			if (removeTabIndex)
			CurrentTabIndex--;
		}

		internal void OpenBracket (StringBuilder sb, bool addTabIndex = true)
		{
			AppendLine (sb, "{");
			if (addTabIndex)
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

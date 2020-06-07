using System.Text;

namespace FigmaSharp
{
    public class FigmaPublicPartialClass : FigmaClassBase
    {
        public FigmaPublicPartialClass()
        {
        }

        public string Namespace { get; set; }
        public string ClassName { get; set; }
        public string BaseClass { get; set; }

        protected void GeneratePublicPartialClass(StringBuilder sb, string className, string baseClassName)
        {
            baseClassName = !string.IsNullOrEmpty(baseClassName) ? $" : {baseClassName}" : string.Empty;
            AppendLine(sb, $"public partial class {className}{baseClassName}");
            OpenBracket(sb);
        }

        protected void ClosePublicPartialClass(StringBuilder sb) => CloseBracket(sb);

        protected override string OnGenerate()
        {
            var sb = new StringBuilder();
            GenerateComments(sb);
            GenerateUsings(sb);
            GenerateNamespace(sb, Namespace);
            GeneratePublicPartialClass(sb, ClassName, BaseClass);
            GenerateConstructor(sb, ClassName);
            AppendLine(sb, $"{InitializeComponentMethodName} ();");
            RemoveTabLevel();
            CloseConstructor(sb);
            ClosePublicPartialClass(sb);
            CloseNamespace(sb);
            return sb.ToString();
        }
    }
}

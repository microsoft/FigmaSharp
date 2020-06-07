using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace FigmaSharp
{
    public class FigmaPartialDesignerClass : FigmaClassBase
    {
        public List<(string memberType, string name)> PrivateMembers = new List<(string memberType, string name)>();
        public string Namespace { get; set; }
        public string ClassName { get; set; }

        public string InitializeComponentContent { get; set; }

        public FigmaPartialDesignerClass()
        {
        }

        protected void GeneratePartialDesignerClass(StringBuilder sb, string className)
        {
            AppendLine(sb, $"partial class {className}");
            OpenBracket(sb);
        }
        protected void ClosePartialDesignerClass(StringBuilder sb) => CloseBracket(sb);

        protected void GenerateInitializeComponentMethod(StringBuilder sb)
        {
            GenerateMethod(sb, InitializeComponentMethodName);

            if (InitializeComponentContent != null)
            {
                foreach (var line in InitializeComponentContent.Split('\n'))
                {
                    AppendLine(sb, line);
                }
            }
            RemoveTabLevel();
            CloseMethod(sb);
        }

        protected virtual void GenerateMethods(StringBuilder sb)
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
                .Distinct();

            foreach (var member in groupedMembers)
            {
                var items = PrivateMembers
                    .Where(s => s.memberType == member)
                    .Select(s => s.name)
                    .ToArray();

                var separatedValues = string.Join(", ", items);
                AppendLine(sb, $"private {member} {separatedValues};");
            }
            AppendLine(sb);
        }

        protected void CloseInitializeComponentMethod(StringBuilder sb) => CloseBracket(sb);

        protected override string OnGenerate()
        {
            var sb = new StringBuilder();
            GenerateComments(sb);
            GenerateUsings(sb);
            GenerateNamespace(sb, Namespace);
            GeneratePartialDesignerClass(sb, ClassName);
            GenerateMembers(sb);
            GenerateInitializeComponentMethod(sb);
            GenerateMethods(sb);
            ClosePartialDesignerClass(sb);
            CloseNamespace(sb);
            return sb.ToString();
        }
    }
}

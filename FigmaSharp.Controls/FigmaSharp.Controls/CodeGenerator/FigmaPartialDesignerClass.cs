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

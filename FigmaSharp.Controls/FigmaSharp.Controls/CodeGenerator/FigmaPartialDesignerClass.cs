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
using System;

namespace FigmaSharp
{
	public class ClassMember
	{
		public ClassMember(string fullname, string name, bool isWeakReference)
		{
			FullName = fullname;
			Name = name;
            IsWeakReference = isWeakReference;
        }

        public bool IsWeakReference { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
    }

    public class FigmaPartialDesignerClass : FigmaClassBase
    {
        List<ClassMember> privateMembers = new List<ClassMember>();
        public List<ClassMember> PrivateMembers => privateMembers;

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
            GeneratePrivateMembers(sb);
            GenerateWeakMembers(sb);
        }

        const string WeakConstant = "weak";

        string GetWeakPropertyName (string name) => $"{WeakConstant}{name.ToCamelCase()}";

        void GenerateWeakMembers(StringBuilder sb)
        {
            AppendLine(sb, string.Empty);
            AppendComment(sb, "Weak Members");
            //private members
            var groupedMembers = PrivateMembers
                .Where(s => s.IsWeakReference)
                .Select(s => s.FullName)
                .Distinct();

            if (!groupedMembers.Any())
                return;

            foreach (var member in groupedMembers)
            {
                var members = PrivateMembers
                    .Where(s => s.FullName == member && s.IsWeakReference)
                    .ToArray();

                var separatedValues = string.Join(", ", members.Select(s => GetWeakPropertyName(s.Name)));
                AppendLine(sb, $"{nameof(System.WeakReference)}<{member}> {separatedValues};");
            }

            AppendLine(sb, string.Empty);
            AppendComment(sb, "Weak Properties");
            foreach (var member in groupedMembers)
            {
                var members = PrivateMembers
                    .Where(s => s.FullName == member && s.IsWeakReference)
                    .ToArray();

				foreach (var item in members)
				{
                    var snippet = GetWeakMemberSnippet(item.FullName, item.Name, GetWeakPropertyName(item.Name));
					foreach (var line in snippet.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
					{
                        AppendLine (sb, line);
					}
                }
            }

            AppendLine(sb);
        }

        string GetWeakMemberSnippet (string type, string name, string targetName)
            => $"{type} {name} => {targetName}.TryGetTarget(out var target) ? target : null;";

        void GeneratePrivateMembers(StringBuilder sb)
		{
            //private members
            var groupedMembers = PrivateMembers
                .Where(s => !s.IsWeakReference)
                .Select(s => s.FullName)
                .Distinct();

            if (!groupedMembers.Any())
                return;

            foreach (var member in groupedMembers)
            {
                var members = PrivateMembers
                    .Where(s => s.FullName == member && !s.IsWeakReference)
                    .ToArray();

                var separatedValues = string.Join(", ", members.Select(s => s.Name));
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

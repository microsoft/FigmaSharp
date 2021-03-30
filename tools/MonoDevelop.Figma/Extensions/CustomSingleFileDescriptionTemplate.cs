// Authors:
//   Matt Ward <matt.ward@microsoft.com>
//
// Copyright (C) 2021 Microsoft, Corp
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
using System.IO;
using System.Threading.Tasks;
using MonoDevelop.Core.StringParsing;
using MonoDevelop.Core.Text;
using MonoDevelop.Ide.Templates;
using MonoDevelop.Projects;

namespace MonoDevelop.Figma
{
    /// <summary>
    /// Avoid duplicating the formatting code from SingleFileDescriptionTemplate by re-using that class.
    /// </summary>
    class CustomSingleFileDescriptionTemplate : SingleFileDescriptionTemplate
    {
        string fileName;

        public override Task<Stream> CreateFileContentAsync(SolutionFolderItem policyParent, Project project, string language, string fileName, string identifier)
        {
            this.fileName = fileName;
            return base.CreateFileContentAsync(policyParent, project, language, fileName, identifier);
        }

        public override string CreateContent(Project project, Dictionary<string, string> tags, string language)
        {
            return TextFileUtility.ReadAllText(fileName);
        }

        protected override string ProcessContent(string content, IStringTagModel tags)
        {
            return content;
        }
    }
}

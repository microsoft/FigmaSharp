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
using System.Text;

using NUnit.Framework;

namespace FigmaSharp.Tests
{
    [TestFixture]
	public class CodeGeneratorTests
	{
        [Test]
        public void ManifestTest ()
        {
            var DocumentUrl = "https://www.figma.com/file/fKugSkFGdwOF4vDsPGnJee/";
            var RemoteApiVersion = "1.1";
            var DocumentVersion = "0.3";
            var dateNow = DateTime.Now;

            var manifest = new FigmaManifest () {
                Date = dateNow,
                FileId = DocumentUrl,
                DocumentVersion = DocumentVersion,
                RemoteApiVersion = RemoteApiVersion,
            };
            var builder = new StringBuilder ();
            manifest.ToComment (builder);
            Assert.IsNotEmpty (builder.ToString ());

            var tempDirectory = Path.GetTempPath ();
            var file = Path.Combine (tempDirectory, "manifest");

            if (File.Exists (file))
                File.Delete (file);

            manifest.Save (file);

			var copy = FigmaManifest.FromFilePath (file);
            Assert.AreEqual (copy.DocumentVersion, manifest.DocumentVersion);
            Assert.AreEqual (copy.Date, manifest.Date);
            Assert.AreEqual (copy.FileId, manifest.FileId);
            Assert.AreEqual (copy.RemoteApiVersion, manifest.RemoteApiVersion);

            if (File.Exists (file))
                File.Delete (file);
        }

        [Test]
        public void PartialDesignerClassGenerationTest()
        {
            var codeGenerator = new FigmaPartialDesignerClass();
            codeGenerator.Manifest = new FigmaManifest()
            {
                Date = DateTime.Now,
                FileId = "https://www.figma.com/file/fKugSkFGdwOF4vDsPGnJee/",
                DocumentVersion = "0.1f",
                RemoteApiVersion = "1.1"
            };

            codeGenerator.Usings.Add("AppKit");
            codeGenerator.ClassName = "MyGeneratedCustomView";
            codeGenerator.Namespace = "LocalFile.Cocoa";
            var classCode = codeGenerator.Generate();
            Assert.IsNotEmpty(classCode);
        }


        [Test]
        public void PartialDesignerWeak()
        {
            var codeGenerator = new FigmaPartialDesignerClass();
            codeGenerator.Manifest = new FigmaManifest()
            {
                Date = DateTime.Now,
                FileId = "https://www.figma.com/file/fKugSkFGdwOF4vDsPGnJee/",
                DocumentVersion = "0.1f",
                RemoteApiVersion = "1.1"
            };
            codeGenerator.Usings.Add("System");
            codeGenerator.Usings.Add("AppKit");
            codeGenerator.ClassName = "MyGeneratedCustomView";
            codeGenerator.Namespace = "LocalFile.Cocoa";

            codeGenerator.PrivateMembers.Add (new ClassMember ("AppKit.NSComboBox", "ComboBox", true));

            var classCode = codeGenerator.Generate();
            Assert.IsNotEmpty(classCode);
        }

        [Test]
        public void PublicPartialClassGenerationTest()
        {
            var codeGenerator = new FigmaPublicPartialClass();
            codeGenerator.Usings.Add("AppKit");
            codeGenerator.ClassName = "MyGeneratedCustomView";
            codeGenerator.Namespace = "LocalFile.Cocoa";
            codeGenerator.BaseClass = "AppKit.NSView";
            var classCode = codeGenerator.Generate();
            Assert.IsNotEmpty(classCode);
        }
    }
}

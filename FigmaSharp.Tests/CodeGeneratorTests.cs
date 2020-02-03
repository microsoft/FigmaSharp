using System;
using NUnit;
using NUnit.Framework;
using FigmaSharp;
using System.Text;
using System.IO;

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
            var DocumentVersion = 0.3f;
            var dateNow = DateTime.Now;

            var manifest = new FigmaManifest () {
                Date = dateNow,
                FileId = DocumentUrl,
                DocumentVersion = DocumentVersion,
                RemoteApiVersion = RemoteApiVersion,
            };
            var builder = new StringBuilder ();
            manifest.ToComment (builder);
            Assert.IsNotNullOrEmpty (builder.ToString ());

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
        public void PartialDesignerClassGenerationTest ()
        {
            var codeGenerator = new FigmaPartialDesignerClass ();
            codeGenerator.Manifest = new FigmaManifest () {
                Date = DateTime.Now,
                DocumentUrl = "https://www.figma.com/file/fKugSkFGdwOF4vDsPGnJee/",
                DocumentVersion = 0.1f,
                RemoteApiVersion = "1.1"
            };

            codeGenerator.Usings.Add ("AppKit"); 
            codeGenerator.ClassName = "MyGeneratedCustomView";
            codeGenerator.Namespace = "LocalFile.Cocoa";
            var classCode = codeGenerator.Generate ();
            Assert.IsNotNullOrEmpty (classCode);
        }

        [Test]
        public void PublicPartialClassGenerationTest ()
        {
            var codeGenerator = new FigmaPublicPartialClass ();
            codeGenerator.Usings.Add ("AppKit");
            codeGenerator.ClassName = "MyGeneratedCustomView";
            codeGenerator.Namespace = "LocalFile.Cocoa";
            codeGenerator.BaseClass = "AppKit.NSView";
            var classCode = codeGenerator.Generate ();
            Assert.IsNotNullOrEmpty (classCode);
        }
    }
}

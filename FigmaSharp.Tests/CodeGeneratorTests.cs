using System;
using NUnit;
using NUnit.Framework;
using FigmaSharp;

namespace FigmaSharp.Tests
{
	[TestFixture]
	public class CodeGeneratorTests
	{
        [Test]
        public void PartialDesignerClassGenerationTest ()
        {
            var codeGenerator = new FigmaPartialDesignerClass ();
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

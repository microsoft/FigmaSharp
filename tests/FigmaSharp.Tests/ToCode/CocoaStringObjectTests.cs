// Authors:
//   jmedrano <josmed@microsoft.com>
//
// Copyright (C) 2020 Microsoft, Corp
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
using FigmaSharp.Cocoa.CodeGeneration;
using FigmaSharp.Services;
using NUnit.Framework;

namespace FigmaSharp.Tests.ToCode
{
    [TestFixture(TestName = "CocoaStringObject")]
    public class CocoaStringObjectTests
    {
        public CocoaStringObjectTests()
        {
        }

        [Test]
        public void CocoaNodeStringObject_GenerationTest()
        {
            var node = new CodeNode(new Models.FigmaNode (), "hello");
            var shapeLayerObject = new CocoaNodeStringObject (node, default (Type));

            Assert.AreEqual(node.Name, shapeLayerObject.ToString());
            shapeLayerObject.AddChild("test1");
            Assert.AreEqual($"{node.Name}.test1", shapeLayerObject.ToString());
            shapeLayerObject.AddArrayChild("Views", 2);
            Assert.AreEqual($"{node.Name}.test1.Views[2]", shapeLayerObject.ToString());
            shapeLayerObject.AddCast(typeof(System.String));
            Assert.AreEqual($"(System.String){node.Name}.test1.Views[2]", shapeLayerObject.ToString());
            shapeLayerObject.AddEnclose();
            Assert.AreEqual($"((System.String){node.Name}.test1.Views[2])", shapeLayerObject.ToString());
        }

        [Test]
        public void CocoaStringObject_GenerationTest()
        {
            var shapeLayerObject = new CocoaStringObject("test", default(Type));
            Assert.AreEqual("test", shapeLayerObject.ToString());
            shapeLayerObject.AddChild("test1");
            Assert.AreEqual("test.test1", shapeLayerObject.ToString());
            shapeLayerObject.AddArrayChild ("Views",2);
            Assert.AreEqual("test.test1.Views[2]", shapeLayerObject.ToString());
            shapeLayerObject.AddCast(typeof(System.String));
            Assert.AreEqual("(System.String)test.test1.Views[2]", shapeLayerObject.ToString());
            shapeLayerObject.AddEnclose();
            Assert.AreEqual("((System.String)test.test1.Views[2])", shapeLayerObject.ToString());
        }
    }
}
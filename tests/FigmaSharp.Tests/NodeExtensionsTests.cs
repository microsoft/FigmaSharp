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

using NUnit.Framework;

using FigmaSharp.Controls;
using FigmaSharp.Models;

namespace FigmaSharp.Tests
{
    [TestFixture]
    public class NodeExtensionsTests
    {
        [Test]
        public void HasNodeImageName ()
        {
            var node = new FigmaFrame();
            node.name = "!image";
            Assert.IsTrue (node.HasNodeImageName ());
            node.name = "image";
            Assert.IsTrue(node.HasNodeImageName());
            node.name = "imag";
            Assert.IsFalse(node.HasNodeImageName());
        }

        [Test]
        public void IsThemedImageViewNode()
        {
            var node = new FigmaFrame();
            node.name = "!image \"AboutHero\" !file:\"img-about-hero\"";

            var themeImage = CocoaThemes.Dark;
            //without nodes
            var theme = new FigmaFrame() { name = $"!theme:\"{themeImage}\"" };
            Assert.IsFalse(theme.IsThemedImageViewNode(out _));
            theme.Parent = node;
            node.children = new FigmaNode[] { theme };

            Assert.IsTrue(theme.IsThemedImageViewNode(out var value));
            Assert.AreEqual(themeImage, value);

            var themeImage2 = CocoaThemes.Light;
            //without nodes
            var theme2 = new FigmaFrame() { name = $"!theme:\"{themeImage2}\"" };
            Assert.IsFalse(theme2.IsThemedImageViewNode(out _));
            theme2.Parent = node;
            node.children = new FigmaNode[] { theme, theme2 };

            Assert.IsTrue(theme.IsThemedImageViewNode(out var value2));
            Assert.AreEqual(themeImage, value2);
            Assert.IsTrue(theme2.IsThemedImageViewNode(out var value3));
            Assert.AreEqual(themeImage2, value3);
        }

        [Test]
        public void IsSingleImageViewNode()
        {
            var node = new FigmaFrame();
            node.name = "!image \"AboutHero\" !file:\"img-about-hero\"";

            //without nodes
            Assert.IsTrue(node.IsSingleImageViewNode());

            //we add a random node
            var a11node = new FigmaFrame() { name = "!a11y" };
            node.children = new FigmaNode[] { a11node };
            Assert.IsTrue(node.IsSingleImageViewNode());

            var theme = new FigmaFrame() { name = "!theme:\"Light\"" };
            node.children = new FigmaNode[] { a11node, theme };

            Assert.IsFalse(node.IsSingleImageViewNode());
        }

        [Test]
        public void NodeAttributedValue ()
        {
            var nodeType = "image";
            var nodeName = "AboutHero";
            var attribute = "file";
            var attributeValue = "img-about-hero";

            //whithout children
            var node = new FigmaFrame();
            node.name = $"!{nodeType} \"{nodeName}\" !{attribute}:\"{attributeValue}\"";
            Assert.IsTrue(node.TryGetAttributeValue (attribute, out var value));
            Assert.AreEqual(attributeValue, value);

            Assert.IsTrue(node.TryGetNodeCustomName (out value));
            Assert.AreEqual(nodeName, value);
            Assert.AreEqual(nodeType, node.GetNodeTypeName ());
        }
    }
}

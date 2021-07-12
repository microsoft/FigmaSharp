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

using FigmaSharp.Controls.Cocoa;
using FigmaSharp.Models;

namespace FigmaSharp.Tests
{
    [TestFixture]
    public class NodeAccessibility
    {
        FigmaFrame AddAccessibilityNode (FigmaFrame parentNode)
        {
            var a11yNode = new FigmaFrame() { name = $"{AccessibilityNodeExtensions.a11yNodeName}" };
            return AddNode(a11yNode, parentNode);
        }

        FigmaFrame AddNode(FigmaFrame node, FigmaFrame parentNode)
        {
            parentNode.children = new FigmaNode[] { node };
            node.Parent = parentNode;
            return node;
        }

        [Test]
        public void AccessibilityLabelTitleDescription()
        {
            var node = new FigmaFrame() { name = "!image" };
            var a11yNode = AddAccessibilityNode(node);

            var label = "hello";
            var a11label = new FigmaFrame() { name = CreateParameter (AccessibilityNodeExtensions.a11yLabel, label) };
            AddNode(a11label, a11yNode);

            Assert.IsTrue(node.TrySearchAccessibilityLabel(out var outlabel));
            Assert.AreEqual(label, outlabel);

            var help = "this help";
            var a11lhelp = new FigmaFrame() { name = CreateParameter(AccessibilityNodeExtensions.a11yHelp, help) };
            AddNode(a11lhelp, a11yNode);

            Assert.IsTrue(node.TrySearchAccessibilityHelp(out var outhelp));
            Assert.AreEqual(help, outhelp);
        }

        [Test]
        public void HasAccessibilityNode ()
        {
            var node = new FigmaFrame();
            node.name = $"!image";
            Assert.IsNull (node.GetAccessibilityNode());
            //we add a random node
            var a11yNode = AddAccessibilityNode(node);
            Assert.AreEqual(a11yNode, node.GetAccessibilityNode ());
        }

        string CreateParameter(string param, string value, bool isDisabled = false) => string.Format("{2}{0}:\"{1}\"", param, value, isDisabled ? "!": "");

        [Test]
        public void HasAccessibilityGroup()
        {
            var node = new FigmaFrame();
            node.name = $"!image";
            Assert.IsNull(node.GetAccessibilityNode());
            //we add a random node
            var a11yNode = AddAccessibilityNode(node);

            var roleGroup = new FigmaFrame() {
                name = CreateParameter(AccessibilityNodeExtensions.a11yRole, AccessibilityNodeExtensions.a11yRoleGroup)
            };
            AddNode(roleGroup, a11yNode);
            Assert.IsTrue(node.IsAccessibilityGroup());
        }

        [Test]
        public void IsAccessibilityEnabled()
        {
            var node = new FigmaFrame();
            node.name = "!image";

            Assert.IsFalse(node.IsAccessibilityEnabled());

            //we add a random node
            var a11yNode = AddAccessibilityNode(node);
            Assert.AreEqual(a11yNode, node.GetAccessibilityNode());
            a11yNode.visible = true;
            Assert.IsTrue(node.IsAccessibilityEnabled());

            a11yNode.visible = false;
            Assert.IsFalse(node.IsAccessibilityEnabled());
        }
    }
}

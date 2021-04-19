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

using System.Linq;

using NUnit.Framework;
using FigmaSharp.Cocoa.PropertyConfigure;
using FigmaSharp.Controls.Cocoa;
using FigmaSharp.PropertyConfigure;
using FigmaSharp.Services;
using System;

namespace FigmaSharp.Tests.ToCode
{
    [TestFixture(TestName = "CodePropertyConfigure")]
    public class CodePropertyConfigureTests : ConvertersTestBase
    {
        [Test]
        public void StackView_Children()
        {
            var parentNode = new CodeNode(StackViewHorizontalNode) { Name = "item1" };
            Assert.NotNull(parentNode.Node);
            var node = new CodeNode(parentNode.Node.GetChildren().FirstOrDefault()) { Name = "item2" };
            var addCode = propertyConfigure.ConvertToCode(PropertyNames.AddChild, node, parentNode, null, null);
            Assert.AreEqual($"item1.AddArrangedSubview (item2);", addCode);

            var constraintsCode = propertyConfigure
                .ConvertToCode(PropertyNames.Constraints, node, parentNode, null, service)
                .Split(new [] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            Assert.AreEqual(constraintsCode.Length, 0);
        }

        [Test]
        public void View_Children()
        {
            var parentNode = new CodeNode(StackViewNode) { Name = "item1" };
            Assert.NotNull(parentNode.Node);
            var node = new CodeNode(parentNode.Node.GetChildren().FirstOrDefault()) { Name = "item2" };

            var addCode = propertyConfigure.ConvertToCode(PropertyNames.AddChild, node, parentNode, null, null);
            Assert.AreEqual($"item1.AddSubview (item2);", addCode);

            var constraintsCode = propertyConfigure
                .ConvertToCode(PropertyNames.Constraints, node, parentNode, null, service)
                .Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            Assert.AreEqual(constraintsCode.Length, 2);
            Assert.AreEqual("item2.LeftAnchor.ConstraintEqualToAnchor (item1.LeftAnchor, 19f).Active = true;", constraintsCode[0]);
            Assert.AreEqual("item2.TopAnchor.ConstraintEqualToAnchor (item1.TopAnchor, 19f).Active = true;", constraintsCode[1]);
        }
    }
}

// Authors:
//   jmedrano <josmed@microsoft.com>
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
using System;
using System.Text;
using FigmaSharp.Cocoa.CodeGeneration;
using FigmaSharp.Controls;
using FigmaSharp.Controls.Cocoa.Converters;
using FigmaSharp.Controls.Cocoa.Services;
using FigmaSharp.Models;
using FigmaSharp.Services;
using NUnit.Framework;

namespace FigmaSharp.Tests.ToCode
{
    [TestFixture(TestName = "BoxView")]
    class BoxTests : CodeTestBase
    {
        [Test]
        public void BoxTest()
        {
            var node = new FigmaInstance()
            {
                 Component = new FigmaComponent() { name = ControlTypeService.Components.Box }, children = new FigmaNode[]
                 {
                     new FigmaText ()
                 }
            };
            var codeNode = new CodeNode(node, "testNode");
            var boxConverter = new BoxConverter();
            var data = boxConverter.ConvertToCode(codeNode, null, CodeService)
                .Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            AssertEquals(data, nameof(AppKit.NSBox.Title), string.Empty);
            AssertEquals(data, nameof(AppKit.NSBox.Hidden), true);
            AssertEquals(data, nameof(AppKit.NSBox.TranslatesAutoresizingMaskIntoConstraints), false);
        }

        [Test]
        public void BoxSeparatorTest()
        {
            var node = new FigmaInstance()
            {
                Component = new FigmaComponent() { name = ControlTypeService.Components.SeparatorHorizontal },
                children = new FigmaNode[]
                 {
                     new FigmaText ()
                 }
            };
            var codeNode = new CodeNode(node, "testNode");
            var boxConverter = new BoxConverter();
            var data = boxConverter.ConvertToCode(codeNode, null, CodeService)
                .Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            AssertEquals(data, nameof(AppKit.NSBox.Title), string.Empty);
            AssertEquals(data, nameof(AppKit.NSBox.Hidden), true);
            AssertEquals(data, nameof(AppKit.NSBox.TranslatesAutoresizingMaskIntoConstraints), false);
            AssertEquals(data, nameof(AppKit.NSBox.BoxType), AppKit.NSBoxType.NSBoxSeparator);
        }

        [Test]
        public void BoxCustomTest()
        {
            var node = new FigmaInstance()
            {
                Component = new FigmaComponent() { name = ControlTypeService.Components.BoxCustom },
                children = new FigmaNode[]
                 {
                     new FigmaText ()
                 }
            };
            var codeNode = new CodeNode(node, "testNode");
            var boxConverter = new BoxConverter();
            var data = boxConverter.ConvertToCode(codeNode, null, CodeService)
                .Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            AssertEquals(data, nameof(AppKit.NSBox.Title), string.Empty);
            AssertEquals(data, nameof(AppKit.NSBox.Hidden), true);
            AssertEquals(data, nameof(AppKit.NSBox.TranslatesAutoresizingMaskIntoConstraints), false);
            AssertEquals(data, nameof(AppKit.NSBox.BoxType), AppKit.NSBoxType.NSBoxCustom);
        }
    }
}

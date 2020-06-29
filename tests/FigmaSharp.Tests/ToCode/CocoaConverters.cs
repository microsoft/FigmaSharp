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
using System.Linq;
using NUnit.Framework;
using FigmaSharp.Services;
using FigmaSharp.Controls;
using FigmaSharp.Controls.Cocoa;
using FigmaSharp.Controls.Cocoa.Services;
using System.Text;
using FigmaSharp.Controls.Cocoa.Converters;

namespace FigmaSharp.Tests.ToCode
{
    [TestFixture]
    public class CocoaConvertersTests
    {
        const string mainNodeName = "Horizontal";

        Converters.NodeConverter[] converters;
        ControlRemoteNodeProvider provider;

        public CocoaConvertersTests ()
        {
           
        }

        [SetUp]
        public void Init()
        {
            FigmaControlsApplication.Init(Resources.PublicToken);
            converters = FigmaControlsContext.Current.GetConverters(true);

            provider = new ControlRemoteNodeProvider();
            provider.Load("sjjkWQHVrvxVDuQnm7AFMS");
        }

        [TestCase (mainNodeName)]
        [TestCase ("Vertical")]
        public void StackView_Orientation(string orientation)
        {
            var stackViewLayer = provider.FindByName(orientation);
            Assert.NotNull(stackViewLayer);

            var service = new NativeViewCodeService(provider, converters);
            var options = new CodeRenderServiceOptions() {
                ScanChildren = false,
                ShowComments = false,
                ShowAddChild = false,
                ShowSize = false,
                ShowConstraints = false
            };

            var builder = new StringBuilder();
            service.GetCode(builder, new CodeNode(stackViewLayer), currentRendererOptions: options);
            Assert.NotNull(builder);
            Assert.True(builder.ToString().Contains($"stackViewView.Orientation = NSUserInterfaceLayoutOrientation.{orientation};"));
        }

        [Test]
        public void StackView_Children()
        {
            var stackViewLayer = provider.FindByName(mainNodeName);
            Assert.NotNull(stackViewLayer);

            var service = new NativeViewCodeService(provider, converters);
            var options = new CodeRenderServiceOptions()
            {
                ShowComments = false,
                ShowSize = false,
                ShowConstraints = false
            };

            var builder = new StringBuilder();
            service.GetCode(builder, new CodeNode(stackViewLayer), currentRendererOptions: options);
            Assert.NotNull(builder);
        }

        [Test]
        public void StackView_Properties()
        {
            var node = provider.FindByName(mainNodeName);
            Assert.NotNull(node);

            var converter = converters
                .OfType<StackViewConverter>()
                .FirstOrDefault();

            Assert.NotNull(converter);
            Assert.IsTrue(converter.CanConvert(node));

            var codeNode = new CodeNode(node) { Name = "stackView" };

            var builder = new StringBuilder();
            converter.ConfigureCodeProperty(StackViewConverter.Properties.Distribution, codeNode, builder);
            Assert.IsNotEmpty(builder.ToString ());

            builder.Clear();
            converter.ConfigureCodeProperty(StackViewConverter.Properties.Spacing, codeNode, builder);
            Assert.IsNotEmpty(builder.ToString());

            builder.Clear();
            converter.ConfigureCodeProperty(StackViewConverter.Properties.EdgeInsets, codeNode, builder);
            Assert.IsNotEmpty(builder.ToString());

            builder.Clear();
            converter.ConfigureCodeProperty(StackViewConverter.Properties.Orientation, codeNode, builder);
            Assert.IsNotEmpty(builder.ToString());
        }

    }
}

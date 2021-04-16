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
using System.Text;

using NUnit.Framework;

using FigmaSharp.Cocoa.Converters;
using FigmaSharp.Cocoa.PropertyConfigure;
using FigmaSharp.Controls.Cocoa;
using FigmaSharp.Controls.Cocoa.Services;
using FigmaSharp.Models;
using FigmaSharp.PropertyConfigure;
using FigmaSharp.Services;

namespace FigmaSharp.Tests.ToCode
{
	public class ConvertersTestBase
    {
        protected FigmaNode StackViewNode => provider.FindByName("StackView");
        protected FigmaNode StackViewHorizontalNode => provider.FindByName("Horizontal");
        protected FigmaNode StackViewVerticalNode => provider.FindByName("Vertical");

        protected Converters.NodeConverter[] converters;
        protected ControlRemoteNodeProvider provider;

        protected CodeRenderService service;
        protected CodePropertyConfigure propertyConfigure;

        [SetUp]
        public void Init()
        {
            FigmaControlsApplication.Init(Resources.PublicToken);
            converters = FigmaControlsContext.Current.GetConverters(true);

            provider = new ControlRemoteNodeProvider();
            provider.Load("6crgoezVRZOEgn5EXOB0Pj");

            propertyConfigure = new CodePropertyConfigure();
            service = new CodeRenderService(provider, converters, propertyConfigure);
        }
    }

    [TestFixture (TestName = "StackView")]
    public class StackViewTests : ConvertersTestBase
    {
        const string mainNodeName = "Horizontal";

        public StackViewTests ()
        {
           
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

            var nodeName = "stackViewView";
            var builder = new StringBuilder();
            service.GetCode(builder, new CodeNode(stackViewLayer, nodeName), currentRendererOptions: options);
            builder.ReplaceDefaultNameTag(nodeName);

            Assert.IsNotEmpty(builder.ToString ());
            Assert.True(builder.ToString().Contains($"{nodeName}.Orientation = NSUserInterfaceLayoutOrientation.{orientation};"));
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
         
            Assert.IsNotEmpty(builder.ToString ());
        }

        [Test]
        public void StackView_Properties()
        {
            var converter = converters.OfType<StackViewConverter>()
              .FirstOrDefault();
            Assert.NotNull(converter);

            var node = provider.FindByName(mainNodeName);
            Assert.NotNull(node);
        
            Assert.IsTrue(converter.CanConvert(node));

            var codeNode = new CodeNode(node);

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

        [Test]
        public void StackView_AddArrangedSubview()
        {
            var codeConfigure = new CodePropertyConfigure();

            var parentName = "parentNode";
            var parentNode = new CodeNode(provider.FindByName(mainNodeName), parentName); ;
            Assert.NotNull(parentNode.Node);
            Assert.IsTrue(parentNode.Node.IsStackView ());

            var nodeName = "currentNode";
            var codeNode = new CodeNode(new FigmaFrame(), nodeName);

            var result = codeConfigure.ConvertToCode(PropertyNames.AddChild, codeNode, parentNode, null, null);

            Assert.AreEqual($"{parentName}.AddArrangedSubview ({nodeName});", result);
        }
    }
}

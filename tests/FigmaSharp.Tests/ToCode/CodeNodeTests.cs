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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FigmaSharp.Cocoa.Converters;
using FigmaSharp.Controls.Cocoa;
using FigmaSharp.Controls.Cocoa.Converters;
using FigmaSharp.Controls.Cocoa.PropertyConfigure;
using FigmaSharp.Controls.Cocoa.Services;
using FigmaSharp.Converters;
using FigmaSharp.Models;
using FigmaSharp.PropertyConfigure;
using FigmaSharp.Services;
using NUnit.Framework;

namespace FigmaSharp.Tests.ToCode
{
    public class TestHelp
    {
        public static readonly CodePropertyConfigureBase CodePropertyConverter = new ControlCodePropertyConfigure();

        static NodeConverter[] allConverters;
        static NodeConverter[] converters;

        public static NodeConverter[] GetConverters(bool includeAll = true)
        {
            if (converters == null)
            {
                converters = new NodeConverter[] {
					// Buttons
					new ButtonConverter (),
                    new StepperConverter (),
                    new SegmentedControlConverter (),

					// Labels
					new LabelConverter (),

					// TextFields
					new TextFieldConverter (),
                    new TextViewConverter (),

					// Selection
					new PopUpButtonConverter (),
                    new ComboBoxConverter (),
                    new CheckBoxConverter (),
                    new RadioConverter (),
                    new SwitchConverter (),
                    new ColorWellConverter(),

					// Status
					new ProgressIndicatorBarConverter (),
                    new ProgressIndicatorCircularConverter (),
                    new SliderLinearConverter(),
                    new SliderCircularConverter(),

					// Containers
					new TabViewConverter (),
                    new BoxConverter (),
                    new DisclosureViewConverter (),

					// Other
					new CustomViewConverter (),
                    new ImageRenderConverter (),
                };
            }

            if (includeAll)
            {
                if (allConverters == null)
                {
                    allConverters =
                        new NodeConverter []
                        {
                             new StackViewConverter (),
            new RegularPolygonConverter (),
            new TextConverter (),
            new LineConverter (),
            new RectangleVectorConverter (),
            new ElipseConverter (),
            new PointConverter (),
            new FrameConverter (),
            new VectorConverter (),
                        }.Concat(converters).ToArray();
                }
                return allConverters;
            }
            else
            {
                return converters;
            }
        }
    }

    public class TestNodeProvider : NodeProvider
    {
        FigmaFileResponse response;
        public TestNodeProvider (FigmaFileResponse response)
        {
            this.response = response;
        }

        public override string GetContentTemplate(string file)
        {
            return string.Empty;
        }

        public override FigmaFileResponse GetFigmaFileResponse()
        {
            return response;
        }

        public override void OnStartImageLinkProcessing(List<ViewNode> imageFigmaNodes)
        {
            
        }
    }

    [TestFixture(TestName = "CodeNode")]
    public class CodeNodeTests
    {
        public CodeNodeTests()
        {
           
        }

           [Test]
        public void CodeNodeTest()
        {
            var fileRespone = new FigmaFileResponse()
            {
                components = new Dictionary<string, FigmaComponent>(),
                document = new FigmaDocument()
                {
                    name = string.Empty,
                    children = new FigmaCanvas[]
                   {
                        new FigmaCanvas ()
                        {
                            name = string.Empty,
                            absoluteBoundingBox = new Rectangle (0,0,10,10),
                            children = new FigmaNode []
                            {
                                new FigmaInstance () {
                                    name = "!placeholder \"myPlaceholder\"",
                                    absoluteBoundingBox = new Rectangle (0,0,10,10),
                                    constraints = new FigmaLayoutConstraint () { horizontal = "CENTER", vertical = "CENTER" },
                                    children = new FigmaNode[]
                                    {
                                        new FigmaFrame () {
                                            name = "children",
                                            absoluteBoundingBox = new Rectangle (0,0,10,10),
                                            constraints = new FigmaLayoutConstraint () { horizontal = "CENTER", vertical = "CENTER" },
                                            children = new FigmaNode[0]
                                        }
                                    }
                                },
                                new FigmaInstance () {
                                    name = "\"myPlaceholder2\"",
                                    absoluteBoundingBox = new Rectangle (0,0,10,10),
                                    constraints = new FigmaLayoutConstraint () { horizontal = "CENTER", vertical = "CENTER" },
                                    children = new FigmaNode[]
                                    {
                                        new FigmaFrame () {
                                            name = "children2",
                                            absoluteBoundingBox = new Rectangle (0,0,10,10),
                                            constraints = new FigmaLayoutConstraint () { horizontal = "CENTER", vertical = "CENTER" },
                                            children = new FigmaNode[0]
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var nodeProvider = new TestNodeProvider(fileRespone);
            nodeProvider.Load();
            var converters = TestHelp.GetConverters();

            var service = new NativeViewCodeService (nodeProvider, converters, TestHelp.CodePropertyConverter);
            var builder = new StringBuilder();

            //var node = nodeProvider.FindByCustomName("myPlaceholder");
            //Assert.NotNull(node);

            //var codeNode = new CodeNode(node);
            //service.GetCode(builder, codeNode);
            //Assert.IsEmpty (builder.ToString ());

            builder.Clear();
            var node = nodeProvider.FindByCustomName("myPlaceholder2");
            Assert.NotNull(node);

            var codeNode = new CodeNode(node);
            service.GetCode(builder, codeNode);
            Assert.IsNotEmpty(builder.ToString());
        }
    }
}

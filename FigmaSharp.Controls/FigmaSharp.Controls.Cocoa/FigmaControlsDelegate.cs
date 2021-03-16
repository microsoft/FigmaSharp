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

using FigmaSharp.Cocoa.PropertyConfigure;
using FigmaSharp.Controls.Cocoa.Converters;
using FigmaSharp.Controls.Cocoa.PropertyConfigure;
using FigmaSharp.Converters;
using FigmaSharp.Models;
using FigmaSharp.PropertyConfigure;

namespace FigmaSharp.Controls.Cocoa
{
    public class FigmaControlsDelegate : IFigmaControlsDelegate
	{
		static readonly CodePropertyConfigureBase codePropertyConverter = new ControlCodePropertyConfigure();
		static readonly ViewPropertyConfigureBase viewPropertySetter = new ViewPropertyConfigure ();

		static NodeConverter[] allConverters;
		static NodeConverter[] converters;

		public CodePropertyConfigureBase GetCodePropertyConverter ()
		  => codePropertyConverter;

		public NodeConverter[] GetConverters (bool includeAll = true)
		{
			if (converters == null) {
				converters = new NodeConverter[] {

					// Keywords
					new CustomViewConverter (),
					new ImageKeywordConverter (),
					new IconKeywordConverter (),
					new PlaceholderKeywordConverter (),

					// Buttons
					new ButtonConverter (),
					new ButtonSymbolConverter(),
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
					new TableViewConverter (),
					new OutlineViewConverter (),
					new BoxConverter (),
					new DisclosureViewConverter (),
				};
			}

			if (includeAll) {
				if (allConverters == null) {
					allConverters = AppContext.Current.GetFigmaConverters ().Concat (converters).ToArray ();
				}
				return allConverters;
			} else {
				return converters;
			}
		}

		public FigmaBundleViewBase GetBundleView (FigmaBundle bundle, string name, FigmaNode figmaNode)
		{
			//TODO: let's disable for now the special logic for about dialog. It needs a review
			//if (figmaNode.IsComponentContainer ())  {
			//	return new FigmaContainerBundleWindow(bundle, name, figmaNode);
			//}

			if (figmaNode is IFigmaNodeContainer nodeContainer) {
				foreach (var figmaInstance in nodeContainer.children) {
					if (figmaInstance.IsWindowOfType (FigmaControlType.Window)) 
						return new FigmaBundleWindow (bundle, name, figmaNode);
					if (figmaInstance.IsWindowOfType (FigmaControlType.WindowSheet))
						return new FigmaBundleWindow (bundle, name, figmaNode);
					if (figmaInstance.IsWindowOfType (FigmaControlType.WindowPanel))
						return new FigmaBundleWindow (bundle, name, figmaNode);
				}
			}
			return new FigmaBundleView (bundle, name, figmaNode);
		}

		public ViewPropertyConfigureBase GetViewPropertySetter() => viewPropertySetter;

	}
}

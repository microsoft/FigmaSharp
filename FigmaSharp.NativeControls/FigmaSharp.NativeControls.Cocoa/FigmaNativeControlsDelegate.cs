/* 
 * CustomTextFieldConverter.cs
 * 
 * Author:
 *   Jose Medrano <josmed@microsoft.com>
 *
 * Copyright (C) 2018 Microsoft, Corp
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to permit
 * persons to whom the Software is furnished to do so, subject to the
 * following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
 * NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
 * USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System.Linq;

using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using FigmaSharp.NativeControls.Cocoa.Converters;

namespace FigmaSharp.NativeControls.Cocoa
{
	public class FigmaNativeControlsDelegate : IFigmaNativeControlsDelegate
	{
		static readonly FigmaCodePropertyConverterBase codePropertyConverter = new NativeControlsPropertyConverter ();

		static readonly FigmaViewPropertySetterBase viewPropertySetter = new FigmaViewPropertySetter ();

		static FigmaViewConverter[] allConverters;
		static FigmaViewConverter[] converters;

		public FigmaCodePropertyConverterBase GetCodePropertyConverter ()
		  => codePropertyConverter;

		public FigmaViewConverter[] GetConverters (bool includeAll = true)
		{
			if (converters == null) {
				converters = new FigmaViewConverter[] {
					new ImageRenderConverter (),
					new StepperConverter (),
					new DisclosureConverter (),
					new SpinnerConverter (),
					new ProgressBarConverter (),
					new CheckConverter (),
					new ComboBoxConverter (),
					new PopUpButtonConverter (),
					new RadioConverter (),
					new ButtonConverter (),
					new TextFieldConverter (),
					new TextViewConverter (),
					new TabViewConverter (),
					new LabelConverter ()
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
			if (figmaNode.IsComponentContainer ())  {
				return new FigmaContainerBundleWindow(bundle, name, figmaNode);
			}

			if (figmaNode is IFigmaNodeContainer nodeContainer) {
				foreach (var figmaInstance in nodeContainer.children) {
					if (figmaInstance.IsWindowOfType (NativeControlType.WindowStandard)) 
						return new FigmaBundleWindow (bundle, name, figmaNode);
					if (figmaInstance.IsWindowOfType (NativeControlType.WindowSheet))
						return new FigmaBundleWindow (bundle, name, figmaNode);
					if (figmaInstance.IsWindowOfType ( NativeControlType.WindowPanel))
						return new FigmaBundleWindow (bundle, name, figmaNode);
				}
			}
			return new FigmaBundleView (bundle, name, figmaNode);
		}

		public FigmaViewPropertySetterBase GetViewPropertySetter() => viewPropertySetter;

	}
}

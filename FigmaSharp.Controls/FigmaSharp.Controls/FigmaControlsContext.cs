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

using FigmaSharp.Converters;
using FigmaSharp.Models;
using FigmaSharp.PropertyConfigure;

namespace FigmaSharp
{
	public class FigmaControlsContext : IFigmaControlsDelegate
    {
        IFigmaControlsDelegate figmaDelegate;

        public FigmaBundleViewBase GetBundleView (FigmaBundle bundle, string name, FigmaNode figmaNode)
			=> figmaDelegate.GetBundleView (bundle, name, figmaNode);

        #region Static Methods

        static FigmaControlsContext current;
        /// <summary>
        /// The shared AppContext for the application.
        /// </summary>
        /// <value>The current.</value>
        public static FigmaControlsContext Current {
            get {
                if (current == null) {
                    current = new FigmaControlsContext ();
                }
                return current;
            }
        }

		internal void Configuration (IFigmaControlsDelegate applicationDelegate)
		{
            figmaDelegate = applicationDelegate;
        }

        public ViewPropertyConfigureBase GetViewPropertySetter()
            => figmaDelegate.GetViewPropertySetter();

        public CodePropertyConfigureBase GetCodePropertyConverter ()
            => figmaDelegate.GetCodePropertyConverter ();

        public NodeConverter[] GetConverters (bool includeAll = true)
             => figmaDelegate.GetConverters (includeAll);

        #endregion
    }
}

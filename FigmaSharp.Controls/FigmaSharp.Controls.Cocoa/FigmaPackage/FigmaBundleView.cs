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

using FigmaSharp.Services;
using FigmaSharp.Models;
using FigmaSharp.Cocoa;
using FigmaSharp.Controls.Cocoa.Services;
using FigmaSharp.Cocoa.CodeGeneration;

namespace FigmaSharp
{
	public class FigmaBundleView : FigmaBundleViewBase
	{
		public FigmaBundleView (FigmaBundle figmaBundle, string viewName, FigmaNode figmaNode) : base (figmaBundle, viewName, figmaNode)
		{
		}

		protected override void OnGetPartialDesignerClass (FigmaPartialDesignerClass partialDesignerClass, CodeRenderService codeRendererService, bool translateLabels)
		{
			if (FigmaNode == null)
				return;

            partialDesignerClass.Usings.Add (nameof (AppKit));

            //restore this state
            var builder = new System.Text.StringBuilder ();

			builder.WritePropertyEquality(Members.This, nameof(AppKit.NSView.TranslatesAutoresizingMaskIntoConstraints), false);

			var options = new CodeRenderServiceOptions() {
                TranslateLabels = translateLabels
            };

            codeRendererService.GetCode (builder, new CodeNode (FigmaNode, null), null, options);

            if (codeRendererService is NativeViewCodeService nativeViewCodeService)
            {
                partialDesignerClass.PrivateMembers.Clear();
                partialDesignerClass.PrivateMembers.AddRange(nativeViewCodeService.PrivateMembers);
            }

            partialDesignerClass.InitializeComponentContent = builder.ToString ();
		}

		protected override void OnGetPublicDesignerClass (FigmaPublicPartialClass publicPartialClass)
		{
			publicPartialClass.Usings.Add (nameof (AppKit));
			publicPartialClass.BaseClass = typeof (AppKit.NSView).FullName;
		}
	}
}

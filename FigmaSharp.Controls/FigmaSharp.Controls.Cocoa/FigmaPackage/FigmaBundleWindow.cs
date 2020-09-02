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

using AppKit;

using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Controls.Cocoa;
using FigmaSharp.Controls.Cocoa.Services;
using FigmaSharp.Cocoa.CodeGeneration;

namespace FigmaSharp
{
    public class FigmaBundleWindow : FigmaBundleViewBase
    {
		const string frameEntity = "frame";

		public FigmaBundleWindow (FigmaBundle figmaBundle, string viewName, FigmaNode figmaNode) : base (figmaBundle, viewName, figmaNode)
		{
		}

		protected override void OnGetPartialDesignerClass (FigmaPartialDesignerClass partialDesignerClass, CodeRenderService codeRendererService, bool translateLabels)
		{
			if (FigmaNode == null)
				return;

			partialDesignerClass.Usings.Add (nameof (AppKit));

			var options = new CodeRenderServiceOptions() { TranslateLabels = translateLabels };
			codeRendererService.Options = options;

			//restore this state
			var builder = new System.Text.StringBuilder ();
			//builder.AppendLine ("//HACK: Temporal Window Frame Size");

			bool centers = false;

			var windowComponent = FigmaNode.GetDialogInstanceFromParentContainer ();
			if (windowComponent != null) {

				if (windowComponent.Options() is IFigmaNodeContainer figmaNodeContainer)
				{
					var title = ((FigmaNode)figmaNodeContainer)
						.FirstChild (s => s.name == "title" && s.visible) as FigmaText;

					if (title != null)
					{
						if (translateLabels)
							builder.WriteTranslatedEquality(Members.This, nameof(NSWindow.Title), title.characters, codeRendererService);
						else
							builder.WritePropertyEquality(Members.This, nameof(NSWindow.Title), title.characters, inQuotes: true);
					}

					if (figmaNodeContainer.HasChildrenVisible("resize"))
					{
						builder.AppendLine(string.Format("{0}.{1} |= {2};",
							Members.This,
							nameof(AppKit.NSWindow.StyleMask),
							AppKit.NSWindowStyle.Resizable.GetFullName()
						));
					}

					if (figmaNodeContainer.HasChildrenVisible ("close"))
					{
						builder.AppendLine(string.Format("{0}.{1} |= {2};",
							Members.This,
							nameof(AppKit.NSWindow.StyleMask),
							AppKit.NSWindowStyle.Closable.GetFullName()
						));
					}

					if (figmaNodeContainer.HasChildrenVisible("min"))
					{
						builder.AppendLine(string.Format("{0}.{1} |= {2};",
							Members.This,
							nameof(AppKit.NSWindow.StyleMask),
							AppKit.NSWindowStyle.Miniaturizable.GetFullName()
						));
					}

					if (figmaNodeContainer.HasChildrenVisible("max") == false)
					{
						builder.AppendLine(string.Format("{0}.{1} ({2}).{3} = {4};",
							Members.This,
							nameof(NSWindow.StandardWindowButton),
							NSWindowButton.ZoomButton.GetFullName(),
							nameof(NSControl.Enabled),
							bool.FalseString.ToLower()
						));
					}
				}
			}

			//Window Frame
			if (FigmaNode is IAbsoluteBoundingBox box && box.absoluteBoundingBox != null) {
				builder.AppendLine();
				builder.WritePropertyEquality (frameEntity, null, nameof (AppKit.NSWindow.Frame), instanciate: true);

				string instance = typeof (CoreGraphics.CGSize).
					GetConstructor (new string[] {
						box.absoluteBoundingBox.Width.ToDesignerString (),
						box.absoluteBoundingBox.Height.ToDesignerString ()
					});
				builder.WritePropertyEquality (frameEntity, nameof (AppKit.NSWindow.Frame.Size), instance, instanciate: false);

				string parameters = $"{frameEntity}, {true.ToDesignerString ()}";

				builder.WriteMethod (Members.This, nameof (AppKit.NSWindow.SetFrame), parameters);
				builder.WritePropertyEquality (Members.This, nameof (AppKit.NSWindow.ContentMinSize), "this.ContentView.Frame.Size");
			}

			codeRendererService.GetCode (builder, new CodeNode(FigmaNode, null), null, options);
			partialDesignerClass.InitializeComponentContent = builder.ToString ();

			if (codeRendererService is NativeViewCodeService nativeViewCodeService) {
				partialDesignerClass.PrivateMembers.Clear();
				partialDesignerClass.PrivateMembers.AddRange(nativeViewCodeService.PrivateMembers);
			}

			if (centers) {
				builder.AppendLine(string.Format("{0}.{1}();",
				Members.This,
				nameof(AppKit.NSWindow.Center)
				));
			}
		}

		protected override void OnGetPublicDesignerClass (FigmaPublicPartialClass publicPartialClass)
		{
			publicPartialClass.Usings.Add (nameof (AppKit));
			publicPartialClass.BaseClass = typeof (AppKit.NSWindow).FullName;
		}
	}
}

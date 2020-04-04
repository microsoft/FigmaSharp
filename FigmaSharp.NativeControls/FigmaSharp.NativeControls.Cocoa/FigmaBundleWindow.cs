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

using System;
using System.Linq;
using AppKit;
using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using FigmaSharp.NativeControls;
using FigmaSharp.Services;

namespace FigmaSharp
{
    public class FigmaBundleWindow : FigmaBundleViewBase
	{
		public FigmaBundleWindow (FigmaBundle figmaBundle, string viewName, FigmaNode figmaNode) : base (figmaBundle, viewName, figmaNode)
		{
		}

		const string frameEntity = "frame";

		protected override void OnGetPartialDesignerClass (FigmaPartialDesignerClass partialDesignerClass, FigmaCodeRendererService codeRendererService)
		{
			if (FigmaNode == null)
				return;

			partialDesignerClass.Usings.Add (nameof (AppKit));

			//restore this state
			var builder = new System.Text.StringBuilder ();
			//builder.AppendLine ("//HACK: Temporal Window Frame Size");

			bool centers = false;

			var windowComponent = FigmaNode.GetDialogInstanceFromParentContainer ();
			if (windowComponent != null) {

				var optionsNode = windowComponent.children.FirstOrDefault(s => s.name == "!options");
				if (optionsNode is IFigmaNodeContainer figmaNodeContainer)
				{
					if (figmaNodeContainer.HasChildrenVisible("title"))
					{
						var title = (FigmaText)figmaNodeContainer.children.FirstOrDefault(s => s.name == "title");

						if (title != null)
							builder.WriteEquality(CodeGenerationHelpers.This, nameof(AppKit.NSWindow.Title), title.characters ?? "", inQuotes: true);
					}

					if (figmaNodeContainer.HasChildrenVisible("resize"))
					{
						builder.AppendLine(string.Format("{0}.{1} |= {2};",
							CodeGenerationHelpers.This,
							nameof(AppKit.NSWindow.StyleMask),
							AppKit.NSWindowStyle.Resizable.GetFullName()
						));
					}

					if (figmaNodeContainer.HasChildrenVisible ("close"))
					{
						builder.AppendLine(string.Format("{0}.{1} |= {2};",
							CodeGenerationHelpers.This,
							nameof(AppKit.NSWindow.StyleMask),
							AppKit.NSWindowStyle.Closable.GetFullName()
						));
					}

					if (figmaNodeContainer.HasChildrenVisible("min"))
					{
						builder.AppendLine(string.Format("{0}.{1} |= {2};",
							CodeGenerationHelpers.This,
							nameof(AppKit.NSWindow.StyleMask),
							AppKit.NSWindowStyle.Miniaturizable.GetFullName()
						));
					}

					if (figmaNodeContainer.HasChildrenVisible("max") == false)
					{
						builder.AppendLine(string.Format("{0}.{1} ({2}).{3} = {4};",
							CodeGenerationHelpers.This,
							nameof(AppKit.NSWindow.StandardWindowButton),
							NSWindowButton.ZoomButton.GetFullName(),
							nameof(NSControl.Enabled),
							bool.FalseString.ToLower()
						));
					}

					builder.AppendLine();
				}
			}

			if (FigmaNode is IAbsoluteBoundingBox box) {
				builder.WriteEquality (frameEntity, null, nameof (AppKit.NSWindow.Frame), instanciate: true);

				string instance = typeof (CoreGraphics.CGSize).
					GetConstructor (new string[] {
						box.absoluteBoundingBox.Width.ToDesignerString (),
						box.absoluteBoundingBox.Height.ToDesignerString ()
					});
				builder.WriteEquality (frameEntity, nameof (AppKit.NSWindow.Frame.Size), instance, instanciate: false);

				string parameters = $"{frameEntity}, {true.ToDesignerString ()}";

				builder.WriteMethod (CodeGenerationHelpers.This, nameof (AppKit.NSWindow.SetFrame), parameters);
				builder.WriteEquality (CodeGenerationHelpers.This, nameof (AppKit.NSWindow.ContentMinSize), "this.ContentView.Frame.Size");

				builder.AppendLine ();
			}

			codeRendererService.GetCode (builder, new FigmaCodeNode(FigmaNode, null), null);
			partialDesignerClass.InitializeComponentContent = builder.ToString ();

			if (centers) {
				builder.AppendLine(string.Format("{0}.{1}();",
				CodeGenerationHelpers.This,
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
